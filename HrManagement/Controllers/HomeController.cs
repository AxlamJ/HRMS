using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HrManagement.Data;
using HrManagement.Filters;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static Azure.Core.HttpHeader;

namespace HrManagement.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly Helpers.Common _common;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, DataContext context, JwtHelper jwtHelper, Helpers.Common common, IConfiguration configuration, IWebHostEnvironment env)
        {
            _logger = logger;
            _context = context;
            _jwtHelper = jwtHelper;
            _common = common;
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password)
        {
            try
            {
                userName = userName.Trim();
                password = password.Trim();
                // Validate input
                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                {
                    return BadRequest(new { StatusCode = HttpStatusCode.BadRequest, Message = "Invalid credentials." });
                }

                // Check for rate limiting using Dapper
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var rateLimitQuery = @"
                    SELECT COUNT(*) 
                    FROM LoginAttempts 
                    WHERE IpAddress = @IpAddress 
                    AND AttemptTime > DATEADD(MINUTE, -15, GETUTCDATE())";

                using var connection = _context.CreateConnection();
                connection.Open();
                
                var loginAttempts = await connection.ExecuteScalarAsync<int>(rateLimitQuery, new { IpAddress = ipAddress });

                if (loginAttempts >= 15)
                {
                    return StatusCode(429, new { StatusCode = HttpStatusCode.TooManyRequests, Message = "Too many login attempts. Please try again later." });
                }

                var userQuery = @"Select 
                                 U.UserId
                                ,U.EmployeeCode
                                ,U.UserName
                                ,U.Email
                                ,U.PhoneNumber
                                ,U.PasswordHash
                                ,U.Salt
                                ,U.FirstName
                                ,U.LastName
                                ,U.Gender
                                ,E.SiteName as UserSites
                                ,U.UserRoles
                                ,U.IsActive
                                ,E.SiteId
                                ,E.DepartmentName
                                ,E.DepartmentSubCategoryName
                                ,U.CreatedById
                                ,U.CreatedBy
                                ,U.CreatedDate
                                ,U.ModifiedById
                                ,U.ModifiedBy
                                ,U.ModifiedDate
                                ,U.LastLoginDate
                                ,E.ProfilePhotoUrl
                                ,R.RoleResources
                                ,R.RoleName
                                from Users U
                                JOIN Employee E ON U.EmployeeCode = E.EmployeeCode
                                Join Roles R ON R.RoleId = U.UserRoles
                                where Lower(U.Email) = Lower(@UserName)";

                var user = new { UserName = userName, Password = password };
                var userdata = await connection.QueryFirstOrDefaultAsync<User>(userQuery, user);

                // Log login attempt using Dapper
                var logAttemptQuery = @"
                    INSERT INTO LoginAttempts (IpAddress, UserName, AttemptTime, IsSuccessful)
                    VALUES (@IpAddress, @UserName, @AttemptTime, @IsSuccessful)";
                
                await connection.ExecuteAsync(logAttemptQuery, new 
                { 
                    IpAddress = ipAddress,
                    UserName = userName,
                    AttemptTime = DateTime.UtcNow,
                    IsSuccessful = userdata != null
                });

                // Check if user exists and is active
                if (userdata == null || !userdata.IsActive)
                {
                    return Unauthorized(new { StatusCode = HttpStatusCode.Unauthorized, Message = "Invalid credentials." });
                }

                var isUserVerified = PasswordHelper.VerifyPassword(user.Password.Trim(), userdata.Salt, userdata.PasswordHash);

                if (isUserVerified)
                {
                    // Generate JWT token
                    var token = _jwtHelper.GenerateToken(user.UserName);

                    // Set secure cookie
                    Response.Cookies.Append("jwtToken", token, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddHours(5),
                        Secure = true,
                        MaxAge = TimeSpan.FromHours(5)
                    });

                    // Set session data
                    SetSessionandCookies(token, userdata);

                    // Update last login time using Dapper
                    var updateLastLoginQuery = @"
                        UPDATE Users 
                        SET LastLoginDate = @LastLoginDate 
                        WHERE UserId = @UserId";
                    
                    await connection.ExecuteAsync(updateLastLoginQuery, new 
                    { 
                        LastLoginDate = DateTime.UtcNow,
                        UserId = userdata.UserId
                    });

                    // Log successful login
                    _logger.LogInformation("User {UserName} logged in successfully from IP {IpAddress}", user.UserName, ipAddress);

                    return Ok(new { StatusCode = HttpStatusCode.OK, jwtToken = token });
                }
                else
                {
                    return Unauthorized(new { StatusCode = HttpStatusCode.Unauthorized, Message = "Invalid credentials." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {UserName}", userName);
                return StatusCode(500, new { StatusCode = HttpStatusCode.InternalServerError, Message = "An error occurred during login." });
            }
        }

        public void SetSessionandCookies(string token, User userdata)
        {
            HttpContext.Session.SetString("jwtToken", token);
            HttpContext.Session.SetString("UserName", userdata.UserName);
            if (!string.IsNullOrEmpty(userdata.ProfilePhotoUrl))
            {
                HttpContext.Session.SetString("ProfilePhoto", userdata.ProfilePhotoUrl);
            }
            HttpContext.Session.SetString("UserId", Convert.ToString(userdata.UserId));
            HttpContext.Session.SetString("Email", userdata.Email);
            if (!string.IsNullOrEmpty(userdata.PhoneNumber))
            {
                HttpContext.Session.SetString("PhoneNumber", userdata.PhoneNumber);
            }
            HttpContext.Session.SetString("FirstName", userdata.FirstName);
            HttpContext.Session.SetString("LastName", userdata.LastName);
            HttpContext.Session.SetString("Gender", userdata.Gender);
            if (!string.IsNullOrEmpty(userdata.UserSites))
            {
                HttpContext.Session.SetString("UserSites", userdata.UserSites);
            }
            if (!string.IsNullOrEmpty(userdata.UserRoles))
            {
                HttpContext.Session.SetString("UserRoles", userdata.UserRoles);
            }
            if (!string.IsNullOrEmpty(userdata.RoleName))
            {
                HttpContext.Session.SetString("UserRoleName", userdata.RoleName);
            }
            if (userdata.EmployeeCode != null && userdata.EmployeeCode != 0)
            {
                HttpContext.Session.SetInt32("EmployeeCode", Convert.ToInt32(userdata.EmployeeCode));
            }
            if (!string.IsNullOrEmpty(userdata.RoleResources))
            {
  //              userdata.RoleResources= @"[
  //{
  //  ""employee-management"": [""manage-employee"", ""manage-site"", ""add-site""],
  //  ""time-management"": [""view-attendance""],
  //  ""leaves-management"": [""leaves-policy"", ""apply-leave"", ""search-leave"", ""approve-leave""],
  //  ""schedule-management"": [""my-schedule"", ""manage-schedule""],
  //  ""administration"": [""manage-departments"", ""manage-positions""],
  //  ""survey-management"": [""manage-survey"", ""my-survey""],
  //  ""news-feed"": [""manage-news-feed"", ""view-news-feed""],
  //  ""settings"": [""permissions""],
  //  ""reports"": [""company-reports""],
  //  ""training-material"": [""view-training-material""]
  //}
//]";
                HttpContext.Session.SetString("RoleResources", userdata.RoleResources);
            }
            if (!string.IsNullOrEmpty(userdata.DepartmentName))
            {
                HttpContext.Session.SetString("Department", userdata.DepartmentName);
            }
            if (!string.IsNullOrEmpty(userdata.DepartmentSubCategoryName))
            {
                HttpContext.Session.SetString("DepartmentSubCategoryName", userdata.DepartmentSubCategoryName);
            }
            if (!string.IsNullOrEmpty(userdata.SiteId))
            {
                HttpContext.Session.SetString("Site", userdata.SiteId);
            }


            //HttpContext.Response.Cookies.Append("jwtToken", token);
            //HttpContext.Response.Cookies.Append("UserName", userdata.UserName);
            //HttpContext.Response.Cookies.Append("UserId", userdata.UserId.ToString());
            //HttpContext.Response.Cookies.Append("Email", userdata.Email);
            //HttpContext.Response.Cookies.Append("PhoneNumber", userdata.PhoneNumber);
            //HttpContext.Response.Cookies.Append("FirstName", userdata.FirstName);
            //HttpContext.Response.Cookies.Append("LastName", userdata.LastName);
            //HttpContext.Response.Cookies.Append("Gender", userdata.Gender);
            //HttpContext.Response.Cookies.Append("UserSites", userdata.UserSites);
            //HttpContext.Response.Cookies.Append("UserRoles", userdata.UserRoles);
            //HttpContext.Response.Cookies.Append("EmployeeCode", userdata.EmployeeCode);

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Response.Cookies.Delete("UserName");
            HttpContext.Response.Cookies.Delete("UserId");
            HttpContext.Response.Cookies.Delete("Email");
            HttpContext.Response.Cookies.Delete("PhoneNumber");
            HttpContext.Response.Cookies.Delete("FirstName");
            HttpContext.Response.Cookies.Delete("LastName");
            HttpContext.Response.Cookies.Delete("Gender");
            HttpContext.Response.Cookies.Delete("UserSites");
            HttpContext.Response.Cookies.Delete("Site");
            HttpContext.Response.Cookies.Delete("RoleResources");
            HttpContext.Response.Cookies.Delete("EmployeeCode");

            return RedirectToAction("Login");
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        //[Authorize]
        public IActionResult Index()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                try
                {
                    return View();

                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogException(ex);
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult Privacy()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult TimeLog()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public async Task<IActionResult> ApplyLeave()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {

                var EmployeeCode = HttpContext.Session.GetInt32("EmployeeCode");

                using var connection = _context.CreateConnection();
                connection.Open();

                var LeavesPolicyQuery = @"SELECT 
                                            LT.LeaveTypeId,
                                            LT.LeaveTypeName,
                                            LP.Id AS PolicyId,
                                            LP.PolicyName,
                                            LP.PolicyDays
                                        FROM LeaveTypes LT
                                        LEFT JOIN LeavesPolicy LP
                                            ON LT.LeaveTypeId = LP.PolicyTypeId
                                        WHERE LP.IsActive = 1 and LT.IsActive = 1;";

                var leavespolicy = await connection.QueryAsync<LeavePolicy>(LeavesPolicyQuery,null);
                var leavespolicylist = leavespolicy.ToList<LeavePolicy>();

                var EmployeeLeavesQuery = @"SELECT
                                                LeaveTypeID,
                                                LeaveTypeName,
                                                SUM(TotalDays) AS AvailedLeaves
                                            FROM Leaves
                                            WHERE 
                                                EmployeeCode = @EmployeeCode
                                                AND LeaveStatusId IN (1, 2)
                                                AND IsActive = 1
                                            GROUP BY 
                                                LeaveTypeID,
                                                LeaveTypeName;";

                var parameters = new { EmployeeCode = EmployeeCode};
                var employeeLeaves = await connection.QueryAsync<EmployeeLeaves>(EmployeeLeavesQuery, parameters);
                var employeeLeavesList = employeeLeaves.ToList<EmployeeLeaves>();
                connection.Close();

                ViewBag.LeavesData = new
                {
                    LeavesPolicy = leavespolicylist,
                    EmployeeLeaves = employeeLeavesList
                };

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }      
        public IActionResult LeavesPolicyManagement()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult MySchedule()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var userRoleString = HttpContext.Session.GetString("UserRoles");

                if (!string.IsNullOrEmpty(userRoleString))
                {
                    var UserRole = Convert.ToInt32(userRoleString);

                    if (UserRole == 1 || UserRole == 2 || UserRole == 3)
                    {
                        var Employees = _common.GetAllAsync<Employee>("Employees", HttpContext).GetAwaiter().GetResult();
                        var Sites = _common.GetAllAsync<Sites>("Sites", HttpContext).GetAwaiter().GetResult();


                        var UserSites = HttpContext.Session.GetString("UserSites");
                        var UserRoles = HttpContext.Session.GetString("UserRoleName");
                        var EmployeeSites = new List<Site>();
                        if (!string.IsNullOrEmpty(UserSites))
                        {
                            EmployeeSites = JsonConvert.DeserializeObject<List<Site>>(UserSites);
                        }

                        if (!string.IsNullOrEmpty(UserRoles) && UserRoles.ToLower().IndexOf("admin") < 0 && UserRoles.ToLower().IndexOf("super admin") < 0)
                        {
                            Employees = new List<Employee>();

                            Employees = _common.GetAllAsync<Employee>("UserSiteEmployees", HttpContext).GetAwaiter().GetResult();
                            Sites = new List<Sites>();
                            foreach (var site in EmployeeSites)
                            {

                                Sites.Add(new Sites { Id = site.id, SiteName = site.name });
                            }
                        }
                        ViewBag.DropDownData = new
                        {
                            Employees = Employees,
                            Sites = Sites
                        };

                        return View("Schedule_Supervisor");
                    }
                    else
                    {
                        var Employees = _common.GetAllAsync<Employee>("Employees", HttpContext).GetAwaiter().GetResult();
                        var Sites = _common.GetAllAsync<Sites>("Sites", HttpContext).GetAwaiter().GetResult();

                        var UserSites = HttpContext.Session.GetString("UserSites");
                        var UserRoles = HttpContext.Session.GetString("UserRoleName");

                        var EmployeeSites = new List<Site>();
                        if (!string.IsNullOrEmpty(UserSites))
                        {
                            EmployeeSites = JsonConvert.DeserializeObject<List<Site>>(UserSites);
                        }
                        if (!string.IsNullOrEmpty(UserRoles) && UserRoles.ToLower().IndexOf("admin") < 0 && UserRoles.ToLower().IndexOf("super admin") < 0)
                        {
                            Employees = new List<Employee>();

                            Employees = _common.GetAllAsync<Employee>("UserSiteEmployees", HttpContext).GetAwaiter().GetResult();
                            Sites = new List<Sites>();
                            foreach (var site in EmployeeSites)
                            {

                                Sites.Add(new Sites { Id = site.id, SiteName = site.name });
                            }
                        }


                        ViewBag.DropDownData = new
                        {
                            Employees = Employees,
                            Sites = Sites
                        };
                        return View();

                    }
                }
                else
                {
                    var Employees = _common.GetAllAsync<Employee>("Employees", HttpContext).GetAwaiter().GetResult();
                    var Sites = _common.GetAllAsync<Sites>("Sites", HttpContext).GetAwaiter().GetResult();

                    ViewBag.DropDownData = new
                    {
                        Employees = Employees,
                        Sites = Sites
                    };
                    return View();

                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public ActionResult ViewAttendance()
        {
            //var http = new HttpClient
            //{
            //    BaseAddress = new Uri("https://office-api.ngteco.com")
            //};

            //var accessToken = HttpContext.Session.GetString("access");

            //if (string.IsNullOrEmpty(accessToken))
            //{
            //    var response = await http.PostAsJsonAsync<object>("/oauth2/api/v1.0/token", new
            //    {
            //        username = "mmuuzzii84@gmail.com",
            //        password = "Calgary123$"
            //    });

            //    var tokenResult = await response.Content.ReadAsStringAsync();

            //    var token = JsonSerializer.Deserialize<TokenResponse>(tokenResult);

            //    accessToken = token.data.access;

            //    HttpContext.Session.SetString("Access", accessToken);
            //}

            //http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //var today = DateTime.Now.ToString("yyyy-MM-dd");
            //var attendance = "";
            //try
            //{
            //    //https://office-api.ngteco.com/att/api/v1.0/timecard/?current=5&pageSize=300&keyword=&abnormal_records=false&date_range=2024-01-01&date_range=2025-01-24

            //    attendance = await http.GetStringAsync($"/att/api/v1.0/timecard/?current=5&pageSize=300&abnormal_records=false&date_range=2024-01-01&date_range=2025-01-23");
            //    //attendance = await http.GetStringAsync($"att/report/api/v1.0/attendance/?current=1&pageSize=20&date_range=2024-01-01&date_range=2025-01-23");
            //    //attendance = await http.GetStringAsync($"/att/api/v1.0/records/aggregation_list/?current=1&pageSize=20&keyword=&date_range=2024-01-01&date_range={today}");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}

            //ViewBag.Data = attendance;

            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {

                var Employees = _common.GetAllAsync<Employee>("Employees", HttpContext).GetAwaiter().GetResult();



                var UserSites = HttpContext.Session.GetString("UserSites");
                var UserRole = HttpContext.Session.GetString("UserRoleName");

                var EmployeeSites = JsonConvert.DeserializeObject<List<Site>>(UserSites);

                if (!string.IsNullOrEmpty(UserRole) && UserRole.ToLower().IndexOf("admin") < 0 && UserRole.ToLower().IndexOf("super admin") < 0)
                {
                    Employees = new List<Employee>();

                    Employees = _common.GetAllAsync<Employee>("UserSiteEmployees", HttpContext).GetAwaiter().GetResult();
                    //foreach (var site in EmployeeSites)
                    //{

                    //    Sites.Add(new Sites { Id = site.id, SiteName = site.name });
                    //}
                }

                ViewBag.DropDownData = new
                {
                    Employees = Employees
                };
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult TimeApproval()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult SearchLeaves()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var Employees = _common.GetAllAsync<Employee>("Employees", HttpContext).GetAwaiter().GetResult();

                var UserSites = HttpContext.Session.GetString("UserSites");
                var UserRole = HttpContext.Session.GetString("UserRoleName");

                var EmployeeSites = JsonConvert.DeserializeObject<List<Site>>(UserSites);

                if (!string.IsNullOrEmpty(UserRole) && UserRole.ToLower().IndexOf("admin") < 0 && UserRole.ToLower().IndexOf("super admin") < 0)
                {
                    Employees = new List<Employee>();

                    Employees = _common.GetAllAsync<Employee>("UserSiteEmployees", HttpContext).GetAwaiter().GetResult();
                    //foreach (var site in EmployeeSites)
                    //{

                    //    Sites.Add(new Sites { Id = site.id, SiteName = site.name });
                    //}
                }


                ViewBag.DropDownData = new
                {
                    Employees = Employees
                };

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult ApproveLeaves()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var Employees = _common.GetAllAsync<Employee>("Employees", HttpContext).GetAwaiter().GetResult();


                var UserSites = HttpContext.Session.GetString("UserSites");
                var UserRole = HttpContext.Session.GetString("UserRoleName");

                var EmployeeSites = JsonConvert.DeserializeObject<List<Site>>(UserSites);

                if (!string.IsNullOrEmpty(UserRole) && UserRole.ToLower().IndexOf("admin") < 0 && UserRole.ToLower().IndexOf("super admin") < 0)
                {
                    Employees = new List<Employee>();

                    Employees = _common.GetAllAsync<Employee>("UserSiteEmployees", HttpContext).GetAwaiter().GetResult();
                    //foreach (var site in EmployeeSites)
                    //{

                    //    Sites.Add(new Sites { Id = site.id, SiteName = site.name });
                    //}
                }

                ViewBag.DropDownData = new
                {
                    Employees = Employees
                };

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult ManageSites()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult AddSite()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult ManageDepartments()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var Departments = _common.GetAllAsync<Department>("Departments", HttpContext).GetAwaiter().GetResult();

                ViewBag.DropDownData = new
                {
                    Departments = Departments
                };

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }  
        
        public IActionResult ManagePositions()
        {
            if (HttpContext.Session.IsAvailable && HttpContext.Session.GetString("UserName") != null)
            {
                var Departments = _common.GetAllAsync<Department>("Departments", HttpContext).GetAwaiter().GetResult();

                ViewBag.DropDownData = new
                {
                    Departments = Departments
                };

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        public IActionResult TrainingMaterial()
        {
              return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new { StatusCode = HttpStatusCode.BadRequest, Message = "Email is required." });
                }

                var userQuery = @"
                    SELECT U.UserId, U.Email, U.FirstName, U.LastName
                    FROM Users U
                    WHERE LOWER(U.Email) = LOWER(@Email)";

                using var connection = _context.CreateConnection();
                connection.Open();
                var user = await connection.QueryFirstOrDefaultAsync<User>(userQuery, new { Email = email });
                connection.Close();

                if (user == null)
                {
                    return Ok(new { StatusCode = HttpStatusCode.OK, Message = "If your email is registered, you will receive password reset instructions." });
                }

                // Generate a new random password
                var newPassword = (GenerateStrongPassword()).Trim();
                var salt = PasswordHelper.GenerateSalt();
                var passwordHash = PasswordHelper.HashPassword(newPassword, salt);

                // Update user's password
                var updateQuery = @"
                    UPDATE Users 
                    SET PasswordHash = @PasswordHash,
                        Salt = @Salt,
                        ModifiedDate = GETUTCDATE()
                    WHERE UserId = @UserId";

                using var updateConnection = _context.CreateConnection();
                updateConnection.Open();
                await updateConnection.ExecuteAsync(updateQuery, new 
                { 
                    PasswordHash = passwordHash,
                    Salt = salt,
                    UserId = user.UserId
                });
                updateConnection.Close();

                // Send email using the existing Email class
                var emailService = new Email(_configuration,_env);
                var emailBody = $@"
                    <html>
                    <body>
                        <h2>Password Reset</h2>
                        <p>Hello {user.FirstName} {user.LastName},</p>
                        <p>Your password has been reset. Here are your new login credentials:</p>
                        <p><strong>Email:</strong> {user.Email}</p>
                        <p><strong>New Password:</strong> {newPassword}</p>
                        <p>Please login with these credentials and change your password immediately. Login Url : <a href=""https://hrms.chestermerephysio.ca/"" target=""_blank"">hrms.chestermerephysio.ca</a></p>
                        <p>Best regards,<br>HR Management System</p><p><img src='cid:signatureImage' alt='Signature' /></p>
                    </body>
                    </html>";

                var from = _configuration["EmailConfigurations:SMTPUserName"];

                await emailService.SendEmailAsync(
                    from, 
                    user.FirstName+" "+ user.LastName,
                    user.Email,
                    null,
                    null,
                    "Password Reset - HR Management System",
                    emailBody
                    ,null,
                    true,
                    ','
                );

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Password reset instructions have been sent to your email." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ForgotPassword for email {Email}", email);
                return StatusCode(500, new { StatusCode = HttpStatusCode.InternalServerError, Message = "An error occurred while processing your request." });
            }
        }

        public static string GenerateStrongPassword(int length = 8)
        {
            if (length < 3)
                throw new ArgumentException("Password length must be at least 3 to avoid special characters at start/end.");

            const string allChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            const string noSpecial = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            const string specialChars = "!@#$%^&*";

            char[] chars = new char[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] data = new byte[length];
                rng.GetBytes(data);

                // First character (no special)
                chars[0] = noSpecial[data[0] % noSpecial.Length];

                // Last character (no special)
                chars[length - 1] = noSpecial[data[length - 1] % noSpecial.Length];

                // Middle characters (can be any)
                for (int i = 1; i < length - 1; i++)
                {
                    chars[i] = allChars[data[i] % allChars.Length];
                }
            }

            return new string(chars);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }

    public class TokenResponse
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public string access { get; set; }
    }
    public class LeavePolicy
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public int PolicyId { get; set; }
        public string PolicyName { get; set; }
        public int PolicyDays { get; set; }
    }  
    
    public class EmployeeLeaves
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public int AvailedLeaves { get; set; }
    }
}
