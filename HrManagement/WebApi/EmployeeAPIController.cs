using ClosedXML.Excel;
using Dapper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using HrManagement.Data;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace HrManagement.WebApi
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAPIController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly Common _common;
        private readonly Email _email;


        public EmployeeAPIController(DataContext context, IWebHostEnvironment webHostEnvironment, Common common, Email email, IConfiguration configuration)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _common = common;
            _email = email;
            _configuration = configuration;
        }


        [HttpPost("UpsertEmployee")]
        public async Task<IActionResult> UpsertEmployee(Employee employee)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                if (employee != null)
                {
                    if (employee.Id == null)
                    {
                        var InsertQuery = @"INSERT INTO Employee 
                                            (
                                               ProfilePhotoUrl
                                              ,SignaturePhotoUrl
                                              ,LicensePhotoUrl
                                              ,EmployeeCode
                                              ,FirstName
                                              ,LastName
                                              ,HiringDate
                                              ,DOB
                                              ,Gender
                                              ,Email
                                              ,PhoneNumber
                                              ,City
                                              ,PostalCode
                                              ,DepartmentId
                                              ,DepartmentName
                                              ,SiteId
                                              ,SiteName
                                              ,Address
                                              ,PositionId
                                              ,PositionName
                                              ,MaritalStatus
                                              ,ManagerId
                                              ,ManagerName
                                              ,AlternativeEmail
                                              ,ProbationDateStart
                                              ,ProbationDateEnd
                                              ,EmploymentStatusId
                                              ,EmploymentStatus
                                              ,EmploymentLevelId
                                              ,EmploymentLevel
                                              ,CreatedById
                                              ,CreatedBy
                                              ,CreatedDate
                                              ,ModifiedById
                                              ,ModifiedBy
                                              ,ModifiedDate
                                              ,Status
                                            )
                                            VALUES 
                                            (
                                               @ProfilePhotoUrl
                                              ,@SignaturePhotoUrl
                                              ,@LicensePhotoUrl
                                              ,@EmployeeCode
                                              ,@FirstName
                                              ,@LastName
                                              ,@HiringDate
                                              ,@DOB
                                              ,@Gender
                                              ,@Email
                                              ,@PhoneNumber
                                              ,@City
                                              ,@PostalCode
                                              ,@DepartmentId
                                              ,@DepartmentName
                                              ,@SiteId
                                              ,@SiteName
                                              ,@Address
                                              ,@PositionId
                                              ,@PositionName
                                              ,@MaritalStatus
                                              ,@ManagerId
                                              ,@ManagerName
                                              ,@AlternativeEmail
                                              ,@ProbationDateStart
                                              ,@ProbationDateEnd
                                              ,@EmploymentStatusId
                                              ,@EmploymentStatus
                                              ,@EmploymentLevelId
                                              ,@EmploymentLevel
                                              ,@CreatedById
                                              ,@CreatedBy
                                              ,@CreatedDate
                                              ,@ModifiedById
                                              ,@ModifiedBy
                                              ,@ModifiedDate
                                              ,@Status
                                            );"
                        ;

                        employee.DOB = DateTime.ParseExact(employee.DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                        if (!string.IsNullOrEmpty(employee.HiringDate))
                        {
                            employee.HiringDate = DateTime.ParseExact(employee.HiringDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        }
                        if (!string.IsNullOrEmpty(employee.ProbationDateStart))
                        {
                            employee.ProbationDateStart = DateTime.ParseExact(employee.ProbationDateStart, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        }
                        if (!string.IsNullOrEmpty(employee.ProbationDateEnd))
                        {
                            employee.ProbationDateEnd = DateTime.ParseExact(employee.ProbationDateEnd, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        }

                        employee.CreatedById = loggedinUserId;
                        employee.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        employee.CreatedDate = DateTime.UtcNow;
                        employee.ModifiedById = loggedinUserId;
                        employee.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        employee.ModifiedDate = DateTime.UtcNow;
                        //employee.Status = 1;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(InsertQuery, employee);
                        connection.Close();
                    }
                    else
                    {
                        using var _connection = _context.CreateConnection();
                        _connection.Open();
                        var employeequery = "Select * from Employee Where Email = @Email and EmployeeCode != @EmployeeCode";
                        var _params = new { Email = employee.Email, EmployeeCode = employee.EmployeeCode };
                        var employeedata = await _connection.QueryFirstOrDefaultAsync<Employee>(employeequery, _params);
                        _connection.Close();
                        if (employeedata != null)
                        {
                            return StatusCode(409, new
                            {
                                StatusCode = 409,
                                Message = "This Email is already taken."
                                //Message = "Site created successfully!",
                                //Data = new { Id = productId }
                            });

                        }
                        else
                        {

                            var UpdateQuery = @"UPDATE Employee
                                            SET 
                                               ProfilePhotoUrl = @ProfilePhotoUrl
                                              ,SignaturePhotoUrl = @SignaturePhotoUrl
                                              ,LicensePhotoUrl = @LicensePhotoUrl
                                              ,EmployeeCode = @EmployeeCode
                                              ,FirstName = @FirstName
                                              ,LastName = @LastName
                                              ,HiringDate = @HiringDate
                                              ,DOB = @DOB
                                              ,Gender = @Gender
                                              ,Email = @Email
                                              ,PhoneNumber = @PhoneNumber
                                              ,City = @City
                                              ,PostalCode = @PostalCode
                                              ,DepartmentId = @DepartmentId
                                              ,DepartmentName = @DepartmentName
                                              ,SiteId = @SiteId
                                              ,SiteName = @SiteName
                                              ,Address = @Address
                                              ,PositionId = @PositionId
                                              ,PositionName = @PositionName
                                              ,MaritalStatus = @MaritalStatus
                                              ,ManagerId = @ManagerId
                                              ,ManagerName = @ManagerName
                                              ,AlternativeEmail = @AlternativeEmail
                                              ,ProbationDateStart = @ProbationDateStart
                                              ,ProbationDateEnd = @ProbationDateEnd
                                              ,EmploymentStatusId = @EmploymentStatusId
                                              ,EmploymentStatus = @EmploymentStatus
                                              ,EmploymentLevelId = @EmploymentLevelId
                                              ,EmploymentLevel = @EmploymentLevel
                                              ,ModifiedById = @ModifiedById
                                              ,ModifiedBy = @ModifiedBy
                                              ,ModifiedDate = @ModifiedDate
                                              ,Status = @Status
                                               WHERE Id = @Id";

                            employee.DOB = DateTime.ParseExact(employee.DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                            if (!string.IsNullOrEmpty(employee.HiringDate))
                            {
                                employee.HiringDate = DateTime.ParseExact(employee.HiringDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            }
                            if (!string.IsNullOrEmpty(employee.ProbationDateStart))
                            {
                                employee.ProbationDateStart = DateTime.ParseExact(employee.ProbationDateStart, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            }
                            if (!string.IsNullOrEmpty(employee.ProbationDateEnd))
                            {
                                employee.ProbationDateEnd = DateTime.ParseExact(employee.ProbationDateEnd, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            }

                            employee.ModifiedById = loggedinUserId;
                            employee.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                            employee.ModifiedDate = DateTime.UtcNow;
                            employee.Status = 1;
                            using var connection = _context.CreateConnection();
                            connection.Open();
                            await connection.ExecuteAsync(UpdateQuery, employee);


                            var userupdatequery = @"Update User set Email = @Email where EmployeeCode = @EmployeeCode";
                            await connection.ExecuteAsync(userupdatequery, employee);
                            connection.Close();

                            await SendProfileUpdateEmail(employee.EmployeeCode.ToString());
                        }
                    }


                }

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    //Message = "Site created successfully!",
                    //Data = new { Id = productId }
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
            //return Request.CreateResponse(HttpStatusCode.OK, new { StickerAlreadyExist = StickerAlreadyExist });

        }




        [HttpPost("AddNewEmployee")]
        public async Task<IActionResult> AddNewEmployee(Employee employee)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                if (employee != null)
                {
                    if (employee.Id == null)
                    {
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        var employeequery = "Select * from Employee Where Email = @Email";
                        var __params = new { Email = employee.Email };
                        var employeedata = await connection.QueryFirstOrDefaultAsync<Employee>(employeequery, __params);

                        //var _employeequery = "Select * from Employee Where EmployeeCode = @EmployeeCode";
                        //var _params = new { EmployeeCode = employee.EmployeeCode };
                        //var _employeedata = await connection.QueryFirstOrDefaultAsync<Employee>(_employeequery, _params);

                        var ___employeequery = "Select * from Employee Where DOB = @DOB OR EmployeeCode = @EmployeeCode";
                       // var DOB = DateTime.ParseExact(employee.DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                       
                       // var ___params = new { EmployeeCode = employee.EmployeeCode, DOB = DOB };
                       
                        //var ___employeedata = await connection.QueryFirstOrDefaultAsync<Employee>(___employeequery, ___params);
                       
                        connection.Close();

                        if (employeedata != null)
                        {
                            return StatusCode(409, new
                            {
                                StatusCode = 409,
                                Message = "This Email is already taken."
                                //Message = "Site created successfully!",
                                //Data = new { Id = productId }
                            });

                        }
                        //else if (_employeedata != null)
                        //{
                        //    return StatusCode(409, new
                        //    {
                        //        StatusCode = 409,
                        //        Message = "This Employee Code is already taken."
                        //        //Message = "Site created successfully!",
                        //        //Data = new { Id = productId }
                        //    });

                        //}
                        //else if (___employeedata != null)
                        //{
                        //    return StatusCode(409, new
                        //    {
                        //        StatusCode = 409,
                        //        Message = "Employee with same details already exists in the system."
                        //        //Message = "Site created successfully!",
                        //        //Data = new { Id = productId }
                        //    });

                        //}
                        else
                        {
                            var InsertQuery = @"

                                            SELECT @EmployeeCode = ISNULL(MAX(EmployeeCode), 0) + 1 FROM Employee;

                                            INSERT INTO Employee 
                                            (
                                               EmployeeCode
                                              ,FirstName
                                              ,LastName
                                              ,HiringDate
                                              ,DOB
                                              ,Gender
                                              ,Email
                                              ,DepartmentId
                                              ,DepartmentName
                                              ,DepartmentSubCategoryId
                                              ,DepartmentSubCategoryName
                                              ,EmploymentStatusId
                                              ,EmploymentStatus
                                              ,SiteId
                                              ,SiteName
                                              ,MaritalStatus
                                              ,ManagerId
                                              ,ManagerName
                                              ,CreatedById
                                              ,CreatedBy
                                              ,CreatedDate
                                              ,ModifiedById
                                              ,ModifiedBy
                                              ,ModifiedDate
                                              ,Status
                                            )
                                            VALUES 
                                            (
                                               @EmployeeCode
                                              ,@FirstName
                                              ,@LastName
                                              ,@HiringDate
                                              ,@DOB
                                              ,@Gender
                                              ,@Email
                                              ,@DepartmentId
                                              ,@DepartmentName
                                              ,@DepartmentSubCategoryId
                                              ,@DepartmentSubCategoryName
                                              ,@EmploymentStatusId
                                              ,@EmploymentStatus
                                              ,@SiteId
                                              ,@SiteName
                                              ,@MaritalStatus
                                              ,@ManagerId
                                              ,@ManagerName
                                              ,@CreatedById
                                              ,@CreatedBy
                                              ,@CreatedDate
                                              ,@ModifiedById
                                              ,@ModifiedBy
                                              ,@ModifiedDate
                                              ,@Status
                                            );

                                            SELECT EmployeeCode FROM Employee Where Id = (SELECT CAST(SCOPE_IDENTITY() as int))";

                            if (!string.IsNullOrWhiteSpace(employee.DOB))
                            {
                                if (DateTime.TryParseExact(
                                    employee.DOB,
                                    "dd/MM/yyyy",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out DateTime parsedDOB))
                                {
                                    employee.DOB = parsedDOB.ToString("yyyy-MM-dd");
                                }
                                else
                                {
                                    employee.DOB = null;
                                }
                            }
                            else
                            {
                                employee.DOB = null;
                            }
                            if (!string.IsNullOrEmpty(employee.HiringDate))
                            {
                                employee.HiringDate = DateTime.ParseExact(employee.HiringDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            }

                            employee.CreatedById = loggedinUserId;
                            employee.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                            employee.CreatedDate = DateTime.UtcNow;
                            employee.ModifiedById = loggedinUserId;
                            employee.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                            employee.ModifiedDate = DateTime.UtcNow;
                            //employee.Status = 1;
                            using var _connection = _context.CreateConnection();
                            _connection.Open();
                            employee.EmployeeCode = await _connection.ExecuteScalarAsync<int>(InsertQuery, employee);

                            var NotificationQuery = @"INSERT INTO Notifications 
                                                    (
                                                       NotificationType
                                                      ,NotificationDescription
                                                      ,NotificationUrl
                                                      ,EmployeeCode
                                                      ,CreatedDate
                                                      ,IsActive
                                                    )
                                                    VALUES 
                                                    (
                                                       @NotificationType
                                                      ,@NotificationDescription
                                                      ,@NotificationUrl
                                                      ,@EmployeeCode
                                                      ,@CreatedDate
                                                      ,@IsActive
                                                    );";

                            var notif = new Notifications();

                            notif.EmployeeCode = employee.EmployeeCode;
                            notif.NotificationType = "Profile";
                            notif.NotificationDescription = "Complete your Profile by visiting my profile.";
                            notif.NotificationUrl = "Employee/MyProfile";
                            notif.CreatedDate = DateTime.UtcNow;
                            notif.IsActive = true;

                            await _connection.ExecuteAsync(NotificationQuery, notif);

                            //if (employee.CreateUser || employee.Status == 2)
                            if (employee.CreateUser)
                            {
                                var UserInsertUserQuery = @"INSERT INTO Users 
                                            (
                                              EmployeeCode,
                                              UserName,
                                              Email,
                                              PasswordHash,
                                              Salt,
                                              FirstName,
                                              LastName,
                                              Gender,
                                              UserRoles,
                                              IsActive,
                                              CreatedById,
                                              CreatedBy,
                                              CreatedDate,
                                              ModifiedById,
                                              ModifiedBy,
                                              ModifiedDate
                                            ) 
                                              VALUES 
                                            (
                                              @EmployeeCode,
                                              @Email,
                                              @Email,
                                              @PasswordHash,
                                              @Salt,
                                              @FirstName,
                                              @LastName,
                                              @Gender,
                                              @UserRoles,
                                              @IsActive,
                                              @CreatedById,
                                              @CreatedBy,
                                              @CreatedDate,
                                              @ModifiedById,
                                              @ModifiedBy,
                                              @ModifiedDate
                                             );";


                                var password = GenerateStrongPassword();
                                employee.Salt = PasswordHelper.GenerateSalt();
                                employee.PasswordHash = PasswordHelper.HashPassword(password, employee.Salt);

                                //if(employee.Status == 2 && !employee.CreateUser)
                                //{
                                //    employee.UserRoles = "4";
                                //}
                                //user.DOB = DateTime.ParseExact(user.DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                                employee.CreatedById = loggedinUserId;
                                employee.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                                employee.CreatedDate = DateTime.UtcNow;
                                employee.ModifiedById = loggedinUserId;
                                employee.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                                employee.ModifiedDate = DateTime.UtcNow;
                                employee.IsActive = true;
                                await _connection.ExecuteAsync(UserInsertUserQuery, employee);
                                _connection.Close();

                                var from = _configuration["EmailConfigurations:SMTPUserName"];
                                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "NewEmployeePassword.html");
                                string htmlBody = System.IO.File.ReadAllText(templatePath)
                                                      .Replace("{{UserEmail}}", employee.Email)
                                                      .Replace("{{UserPassword}}", password);

                                await _email.SendEmailAsync(from, employee.FirstName + " " + employee.LastName, employee.Email, null, null, "Login Credentials", htmlBody, null, true, ',');

                            }
                            else
                            {
                                _connection.Close();
                            }


                        }
                    }

                }

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    //Message = "Site created successfully!",
                    //Data = new { Id = productId }
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
            //return Request.CreateResponse(HttpStatusCode.OK, new { StickerAlreadyExist = StickerAlreadyExist });

        }



        //[HttpPost("NotifyNewEmployee")]
        //public async Task<IActionResult> NotifyNewEmployee(string? Email,string EmployeeName)
        //{
        //    try
        //    {
        //        var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
        //        var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
        //        var loggedinUserLastName = HttpContext.Session.GetString("LastName");

        //        if (employee != null)
        //        {
        //            if (employee.Id == null)
        //            {
        //                using var connection = _context.CreateConnection();
        //                var employeequery = "Select * from Employee Where Email = @Email";
        //                var paramas = new { Email = employee.Email };
        //                connection.Open();
        //                var employeedata = await connection.QueryFirstOrDefaultAsync<Employee>(employeequery, paramas);

        //                var _employeequery = "Select * from Employee Where EmployeeCode = @EmployeeCode";
        //                var _paramas = new { EmployeeCode = employee.EmployeeCode };
        //                var _employeedata = await connection.QueryFirstOrDefaultAsync<Employee>(_employeequery, _paramas);
        //                connection.Close();

        //                {
        //                    using var _connection = _context.CreateConnection();
        //                    _connection.Open();

        //                    if (employee.CreateUser || employee.Status == 2)
        //                    {
        //                        var UserInsertUserQuery = @"INSERT INTO Users 
        //                                    (
        //                                      EmployeeCode,
        //                                      UserName,
        //                                      Email,
        //                                      PasswordHash,
        //                                      Salt,
        //                                      FirstName,
        //                                      LastName,
        //                                      Gender,
        //                                      UserRoles,
        //                                      IsActive,
        //                                      CreatedById,
        //                                      CreatedBy,
        //                                      CreatedDate,
        //                                      ModifiedById,
        //                                      ModifiedBy,
        //                                      ModifiedDate
        //                                    ) 
        //                                      VALUES 
        //                                    (
        //                                      @EmployeeCode,
        //                                      @Email,
        //                                      @Email,
        //                                      @PasswordHash,
        //                                      @Salt,
        //                                      @FirstName,
        //                                      @LastName,
        //                                      @Gender,
        //                                      @UserRoles,
        //                                      @IsActive,
        //                                      @CreatedById,
        //                                      @CreatedBy,
        //                                      @CreatedDate,
        //                                      @ModifiedById,
        //                                      @ModifiedBy,
        //                                      @ModifiedDate
        //                                     );";


        //                        var password = GenerateStrongPassword();
        //                        employee.Salt = PasswordHelper.GenerateSalt();
        //                        employee.PasswordHash = PasswordHelper.HashPassword(password, employee.Salt);

        //                        //user.DOB = DateTime.ParseExact(user.DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

        //                        employee.CreatedById = loggedinUserId;
        //                        employee.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
        //                        employee.CreatedDate = DateTime.UtcNow;
        //                        employee.ModifiedById = loggedinUserId;
        //                        employee.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
        //                        employee.ModifiedDate = DateTime.UtcNow;
        //                        employee.IsActive = true;
        //                        await _connection.ExecuteAsync(UserInsertUserQuery, employee);
        //                        _connection.Close();

        //                        var from = _configuration["EmailConfigurations:SMTPUserName"];
        //                        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "NewEmployeePassword.html");
        //                        string htmlBody = System.IO.File.ReadAllText(templatePath)
        //                                              .Replace("{{UserEmail}}", employee.Email)
        //                                              .Replace("{{UserPassword}}", password);

        //                        await _email.SendEmailAsync(from,employee.FirstName+" "+employee.LastName,employee.Email,null,null,"Login Credentials",htmlBody,null,true,',');

        //                    }
        //                    else
        //                    {
        //                        _connection.Close();
        //                    }


        //                }
        //            }

        //        }

        //        return StatusCode(200, new
        //        {
        //            StatusCode = 200,
        //            //Message = "Site created successfully!",
        //            //Data = new { Id = productId }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogger.LogException(ex);
        //        return StatusCode(500, new
        //        {
        //            StatusCode = 500
        //        });
        //    }
        //    //return Request.CreateResponse(HttpStatusCode.OK, new { StickerAlreadyExist = StickerAlreadyExist });

        //}

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


        [HttpPost("UpdateBasicDetails")]
        public async Task<IActionResult> UpdateBasicDetails(Employee employee)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                if (employee != null && employee.Id != null)
                {
                    using var _connection = _context.CreateConnection();
                    _connection.Open();
                    var employeequery = "Select * from Employee Where Email = @Email and EmployeeCode != @EmployeeCode";
                    var _params = new { Email = employee.Email, EmployeeCode = employee.EmployeeCode };
                    var employeedata = await _connection.QueryFirstOrDefaultAsync<Employee>(employeequery, _params);

                    var employeesinquery = "Select * from Employee Where SinNo = @SinNo and EmployeeCode != @EmployeeCode";
                    var employeesindata = await _connection.QueryFirstOrDefaultAsync<Employee>(employeequery, _params);

                    if (employeedata != null)
                    {
                        return StatusCode(409, new
                        {
                            StatusCode = 409,
                            Message = "This Email is already taken."
                            //Message = "Site created successfully!",
                            //Data = new { Id = productId }
                        });

                    }
                    else if (employeesindata != null)
                    {
                        return StatusCode(409, new
                        {
                            StatusCode = 409,
                            Message = "This SIN No. is already taken."
                            //Message = "Site created successfully!",
                            //Data = new { Id = productId }
                        });

                    }
                    else
                    {

                        var UpdateQuery = @"UPDATE Employee
                                            SET 
                                               ProfilePhotoUrl = @ProfilePhotoUrl
                                              ,SignaturePhotoUrl = @SignaturePhotoUrl
                                              ,LicensePhotoUrl = @LicensePhotoUrl
                                              ,FirstName = @FirstName
                                              ,LastName = @LastName
                                              ,Gender = @Gender
                                              ,MaritalStatus = @MaritalStatus
                                              ,DOB = @DOB
                                              ,OnBoardingDate = @OnBoardingDate
                                              ,PhoneNumber = @PhoneNumber
                                              ,Email = @Email
                                              ,Country = @Country
                                              ,CountryId = @CountryId
                                              ,City = @City
                                              ,SponsorShip = @SponsorShip
                                              ,WorkEligibility = @WorkEligibility
                                              ,ImmigrationStatus = @ImmigrationStatus
                                              ,Other = @Other
                                              ,SinNo = @SinNo
                                              ,SinDocumentName = @SinDocumentName 
                                              ,SinDocumentUrl = @SinDocumentUrl 
                                              ,ChequeDocumentName = @ChequeDocumentName 
                                              ,ChequeDocumentUrl = @ChequeDocumentUrl 
                                              ,FederalTaxDocumentName = @FederalTaxDocumentName 
                                              ,FederalTaxDocumentUrl = @FederalTaxDocumentUrl 
                                              ,AlbertaTaxDocumentName = @AlbertaTaxDocumentName 
                                              ,AlbertaTaxDocumentUrl = @AlbertaTaxDocumentUrl 
                                               WHERE Id = @Id";

                        employee.DOB = DateTime.ParseExact(employee.DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        employee.OnBoardingDate = DateTime.ParseExact(employee.OnBoardingDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                        employee.ModifiedById = loggedinUserId;
                        employee.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        employee.ModifiedDate = DateTime.UtcNow;
                        employee.Status = 1;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(UpdateQuery, employee);

                        var userupdatequery = @"Update Users set Email = @Email where EmployeeCode = @EmployeeCode";
                        await connection.ExecuteAsync(userupdatequery, employee);

                        connection.Close();
                        await SendProfileUpdateEmail(employee.EmployeeCode.ToString());

                        return StatusCode(200, new
                        {
                            StatusCode = 200,
                            //Message = "Site created successfully!",
                            //Data = new { Id = productId }
                        });
                    }

                }
                else
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500
                    });
                }


            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
            //return Request.CreateResponse(HttpStatusCode.OK, new { StickerAlreadyExist = StickerAlreadyExist });

        }


        [HttpPost("UpdateEmploymentDetails")]
        public async Task<IActionResult> UpdateEmploymentDetails(Employee employee)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                if (employee != null && employee.Id != null)
                {

                    var UpdateQuery = @"UPDATE Employee
                                            SET 
                                               PositionId = @PositionId
                                              ,PositionName = @PositionName
                                              ,HiringDate = @HiringDate
                                              ,SiteId = @SiteId
                                              ,SiteName = @SiteName
                                              ,DepartmentId = @DepartmentId
                                              ,DepartmentName = @DepartmentName
                                              ,DepartmentSubCategoryId = @DepartmentSubCategoryId
                                              ,DepartmentSubCategoryName = @DepartmentSubCategoryName
                                              ,EmploymentStatusId = @EmploymentStatusId
                                              ,EmploymentStatus = @EmploymentStatus
                                              ,EmploymentLevelId = @EmploymentLevelId
                                              ,EmploymentLevel = @EmploymentLevel
                                              ,ManagerId = @ManagerId
                                              ,ManagerName = @ManagerName
                                              ,ProbationDateStart = @ProbationDateStart
                                              ,ProbationDateEnd = @ProbationDateEnd
                                              ,AcceptanceDate = @AcceptanceDate
                                              ,RegistrationDate = @RegistrationDate
                                              ,RegistrationNumber = @RegistrationNumber	
                                              ,LiabilityInsuranceName = @LiabilityInsuranceName	
                                              ,LiabilityInsuranceUrl = @LiabilityInsuranceUrl	
                                              ,RegistrationNumberName = @RegistrationNumberName	
                                              ,RegistrationNumberUrl = @RegistrationNumberUrl	
                                              ,BusinessName = @BusinessName	
                                              ,BusinessEmail = @BusinessEmail	
                                              ,BusinessNumber = @BusinessNumber	
                                              ,BusinessChequeName = @BusinessChequeName	
                                              ,BusinessChequeUrl = @BusinessChequeUrl	
                                              ,ContractorBusinessName = @ContractorBusinessName	
                                              ,ContractorBusinessEmail = @ContractorBusinessEmail	
                                              ,ContractorBusinessNumber = @ContractorBusinessNumber	
                                              ,ContractorBusinessChequeName = @ContractorBusinessChequeName	
                                              ,ContractorBusinessChequeUrl = @ContractorBusinessChequeUrl	
                                               WHERE Id = @Id";

                    if (!string.IsNullOrEmpty(employee.HiringDate))
                    {
                        employee.HiringDate = DateTime.ParseExact(employee.HiringDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    }
                    if (!string.IsNullOrEmpty(employee.ProbationDateStart))
                    {
                        employee.ProbationDateStart = DateTime.ParseExact(employee.ProbationDateStart, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    }
                    if (!string.IsNullOrEmpty(employee.ProbationDateEnd))
                    {
                        employee.ProbationDateEnd = DateTime.ParseExact(employee.ProbationDateEnd, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    }
                    if (!string.IsNullOrEmpty(employee.AcceptanceDate))
                    {
                        employee.AcceptanceDate = DateTime.ParseExact(employee.AcceptanceDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    }
                    if (!string.IsNullOrEmpty(employee.RegistrationDate))
                    {
                        employee.RegistrationDate = DateTime.ParseExact(employee.RegistrationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    }

                    employee.ModifiedById = loggedinUserId;
                    employee.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                    employee.ModifiedDate = DateTime.UtcNow;
                    employee.Status = 1;
                    using var connection = _context.CreateConnection();
                    connection.Open();
                    await connection.ExecuteAsync(UpdateQuery, employee);
                    connection.Close();

                    string query = "SELECT EmployeeCode FROM Employee WHERE Id = @Id";

                    var employeeCode = await connection.QuerySingleAsync<int>(query, new { Id = employee.Id });
                    employee.EmployeeCode = employeeCode;


                    var UserUpdateUserQuery = @"
                                UPDATE Users SET
                                    UserRoles = @UserRoles,       
                                    ModifiedById = @ModifiedById,
                                    ModifiedBy = @ModifiedBy,
                                    ModifiedDate = @ModifiedDate
                                WHERE EmployeeCode = @EmployeeCode;
                            ";

                    int rowsAffected= await connection.ExecuteAsync(UserUpdateUserQuery, employee);
                    connection.Close();

                    await SendProfileUpdateEmail(employee.EmployeeCode.ToString());

                    return StatusCode(200, new
                    {
                        StatusCode = 200,
                        //Message = "Site created successfully!",
                        //Data = new { Id = productId }
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500
                    });
                }


            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
            //return Request.CreateResponse(HttpStatusCode.OK, new { StickerAlreadyExist = StickerAlreadyExist });

        }



        [HttpPost("UpdateConatactDetails")]
        public async Task<IActionResult> UpdateConatactDetails(Employee employee)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                if (employee != null && employee.EmployeeCode != null)
                {

                    var UpdateQuery = @"UPDATE Employee
                                            SET 
                                               OfficePhoneNumber = @OfficePhoneNumber
                                              ,Extension = @Extension
                                              ,PhoneNumber = @PhoneNumber
	                                          ,CountryId = @CountryId
                                              ,Country = @Country
                                              ,City = @City
                                              ,State = @State
                                              ,PostalCode = @PostalCode
                                              ,Address = @Address
                                              ,ModifiedById = @ModifiedById
                                              ,ModifiedBy = @ModifiedBy
                                              ,ModifiedDate	= @ModifiedDate
                                               WHERE EmployeeCode = @EmployeeCode";

                    employee.ModifiedById = loggedinUserId;
                    employee.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                    employee.ModifiedDate = DateTime.UtcNow;
                    employee.Status = 1;
                    using var connection = _context.CreateConnection();
                    connection.Open();
                    await connection.ExecuteAsync(UpdateQuery, employee);
                    connection.Close();
                    await SendProfileUpdateEmail(employee.EmployeeCode.ToString());

                    return StatusCode(200, new
                    {
                        StatusCode = 200,
                        //Message = "Site created successfully!",
                        //Data = new { Id = productId }
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500
                    });

                }



            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
            //return Request.CreateResponse(HttpStatusCode.OK, new { StickerAlreadyExist = StickerAlreadyExist });

        }


        [HttpPost("UpsertEmergencyContact")]
        public async Task<IActionResult> UpsertEmergencyContact(EmployeeEmergencyContacts emergencycontact)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                if (emergencycontact != null)
                {
                    if (emergencycontact.Id == null)
                    {
                        var InsertQuery = @"INSERT INTO EmployeeEmergencyContacts 
                                            (
                                              EmployeeCode
                                             ,FirstName
                                             ,LastName
                                             ,RelationShip
                                             ,OfficePhone
                                             ,MobilePhone
                                             ,CreatedById
                                             ,CreatedBy
                                             ,CreatedDate
                                             ,ModifiedById
                                             ,ModifiedBy
                                             ,ModifiedDate
                                             ,IsActive
                                            )
                                            VALUES 
                                            (
                                              @EmployeeCode
                                             ,@FirstName
                                             ,@LastName
                                             ,@RelationShip
                                             ,@OfficePhone
                                             ,@MobilePhone
                                             ,@CreatedById
                                             ,@CreatedBy
                                             ,@CreatedDate
                                             ,@ModifiedById
                                             ,@ModifiedBy
                                             ,@ModifiedDate
                                             ,@IsActive
                                            );";

                        emergencycontact.CreatedById = loggedinUserId;
                        emergencycontact.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        emergencycontact.CreatedDate = DateTime.UtcNow;
                        emergencycontact.ModifiedById = loggedinUserId;
                        emergencycontact.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        emergencycontact.ModifiedDate = DateTime.UtcNow;
                        emergencycontact.IsActive = true;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(InsertQuery, emergencycontact);
                        connection.Close();
                        await SendProfileUpdateEmail(emergencycontact.EmployeeCode.ToString());

                        return StatusCode(200, new
                        {
                            StatusCode = 200,
                            //Message = "Site created successfully!",
                            //Data = new { Id = productId }
                        });
                    }
                    else
                    {
                        var UpdateQuery = @"UPDATE EmployeeEmergencyContacts
                                            SET 
                                              EmployeeCode = @EmployeeCode 
                                             ,FirstName = @FirstName 
                                             ,LastName = @LastName 
                                             ,RelationShip = @RelationShip 
                                             ,OfficePhone = @OfficePhone 
                                             ,MobilePhone = @MobilePhone  
                                             ,ModifiedById = @ModifiedById 
                                             ,ModifiedBy = @ModifiedBy 
                                             ,ModifiedDate = @ModifiedDate 
                                             ,IsActive = @IsActive 
                                              WHERE Id =  @Id";

                        emergencycontact.ModifiedById = loggedinUserId;
                        emergencycontact.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        emergencycontact.ModifiedDate = DateTime.UtcNow;
                        emergencycontact.IsActive = true;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(UpdateQuery, emergencycontact);
                        connection.Close();
                        await SendProfileUpdateEmail(emergencycontact.EmployeeCode.ToString());

                        return StatusCode(200, new
                        {
                            StatusCode = 200,
                            //Message = "Site created successfully!",
                            //Data = new { Id = productId }
                        });
                    }


                }
                else
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500
                    });
                }

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
            //return Request.CreateResponse(HttpStatusCode.OK, new { StickerAlreadyExist = StickerAlreadyExist });

        }

        [HttpPost("UpsertEducationDetails")]
        public async Task<IActionResult> UpsertEducationDetails(EducationDetails educationDetails)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                if (educationDetails != null)
                {
                    if (educationDetails.Id == null)
                    {
                        var InsertQuery = @"INSERT INTO EmployeeEducation 
                                            (
                                               EmployeeCode
                                              ,CollegeUniversity
                                              ,Degree
                                              ,Major
                                              ,YearGraduated
                                              ,CreatedById
                                              ,CreatedBy
                                              ,CreatedDate
                                              ,ModifiedById
                                              ,ModifiedBy
                                              ,ModifiedDate
                                              ,IsActive
                                            )
                                            VALUES 
                                            (
                                               @EmployeeCode
                                              ,@CollegeUniversity
                                              ,@Degree
                                              ,@Major
                                              ,@YearGraduated
                                              ,@CreatedById
                                              ,@CreatedBy
                                              ,@CreatedDate
                                              ,@ModifiedById
                                              ,@ModifiedBy
                                              ,@ModifiedDate
                                              ,@IsActive     
                                            );";

                        educationDetails.CreatedById = loggedinUserId;
                        educationDetails.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        educationDetails.CreatedDate = DateTime.UtcNow;
                        educationDetails.ModifiedById = loggedinUserId;
                        educationDetails.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        educationDetails.ModifiedDate = DateTime.UtcNow;
                        educationDetails.IsActive = true;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(InsertQuery, educationDetails);
                        connection.Close();
                        await SendProfileUpdateEmail(educationDetails.EmployeeCode.ToString());

                        return StatusCode(200, new
                        {
                            StatusCode = 200,
                            //Message = "Site created successfully!",
                            //Data = new { Id = productId }
                        });
                    }
                    else
                    {
                        var UpdateQuery = @"UPDATE EmployeeEducation
                                            SET 
                                              ,EmployeeCode = @EmployeeCode
                                              ,CollegeUniversity = @CollegeUniversity
                                              ,Degree = @Degree
                                              ,Major = @Major
                                              ,YearGraduated = @YearGraduated
                                              ,ModifiedById = @ModifiedById
                                              ,ModifiedBy = @ModifiedBy
                                              ,ModifiedDate = @ModifiedDate
                                              ,IsActive = @IsActive  
                                              WHERE Id =  @Id";

                        educationDetails.ModifiedById = loggedinUserId;
                        educationDetails.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        educationDetails.ModifiedDate = DateTime.UtcNow;
                        educationDetails.IsActive = true;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(UpdateQuery, educationDetails);
                        connection.Close();
                        await SendProfileUpdateEmail(educationDetails.EmployeeCode.ToString());

                        return StatusCode(200, new
                        {
                            StatusCode = 200,
                            //Message = "Site created successfully!",
                            //Data = new { Id = productId }
                        });
                    }


                }
                else
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500
                    });
                }

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
            //return Request.CreateResponse(HttpStatusCode.OK, new { StickerAlreadyExist = StickerAlreadyExist });

        }

        [HttpPost("UpsertEquipments")]
        public async Task<IActionResult> UpsertEquipments(EmployeeEquipment equipment)
        {
            //Status 1 = Approved, 2 = Pending, 3 = Rejected, 4= Deleted
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");
                if (equipment != null)
                {

                    var query = @"INSERT INTO EmployeeEquipment
                                    (
                                      EmployeeCode, 
                                      EquipmentName, 
                                      Notes, 
                                      Status, 
                                      CreatedById, 
                                      CreatedBy, 
                                      CreatedDate, 
                                      ModifiedById, 
                                      ModifiedBy, 
                                      ModifiedDate, 
                                      IsActive
                                    ) 
                                    VALUES 
                                    (
                                      @EmployeeCode, 
                                      @EquipmentName, 
                                      @Notes, 
                                      @Status, 
                                      @CreatedById, 
                                      @CreatedBy, 
                                      @CreatedDate, 
                                      @ModifiedById, 
                                      @ModifiedBy, 
                                      @ModifiedDate, 
                                      @IsActive
                                    )";


                    equipment.CreatedById = loggedinUserId;
                    equipment.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                    equipment.CreatedDate = DateTime.UtcNow;
                    equipment.ModifiedById = loggedinUserId;
                    equipment.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                    equipment.ModifiedDate = DateTime.UtcNow;
                    equipment.IsActive = true;
                    using var connection = _context.CreateConnection();
                    connection.Open();
                    await connection.ExecuteAsync(query, equipment);
                    connection.Close();

                    await SendProfileUpdateEmail(equipment.EmployeeCode.ToString());


                    return StatusCode(200, new
                    {
                        StatusCode = 200,
                        //Message = "Site created successfully!",
                        //Data = new { Id = productId }
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500
                    });

                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
            //return Request.CreateResponse(HttpStatusCode.OK, new { StickerAlreadyExist = StickerAlreadyExist });

        }

        [HttpPost("UpdateCertDetails")]
        public async Task<IActionResult> UpdateCertDetails(CerificateLicense cert)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");
                if (cert != null)
                {

                    var query = @"INSERT INTO EmployeeLicenseCertification
                                    (
                                      EmployeeCode
                                     ,Name
                                     ,IssuingOrganization
                                     ,CredentialId
                                     ,URL
                                     ,IssueDate
                                     ,ExpirationDate
                                     ,CreatedById
                                     ,CreatedBy
                                     ,CreatedDate
                                     ,ModifiedById
                                     ,ModifiedBy
                                     ,ModifiedDate
                                     ,IsActive
                                    ) 
                                    VALUES 
                                    (
                                      @EmployeeCode
                                     ,@Name
                                     ,@IssuingOrganization
                                     ,@CredentialId
                                     ,@URL
                                     ,@IssueDate
                                     ,@ExpirationDate
                                     ,@CreatedById
                                     ,@CreatedBy
                                     ,@CreatedDate
                                     ,@ModifiedById
                                     ,@ModifiedBy
                                     ,@ModifiedDate
                                     ,@IsActive
                                    )";

                    if (!string.IsNullOrEmpty(cert.IssueDate))
                    {
                        cert.IssueDate = DateTime.ParseExact(cert.IssueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    }
                    if (!string.IsNullOrEmpty(cert.ExpirationDate))
                    {
                        cert.ExpirationDate = DateTime.ParseExact(cert.ExpirationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    }


                    cert.CreatedById = loggedinUserId;
                    cert.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                    cert.CreatedDate = DateTime.UtcNow;
                    cert.ModifiedById = loggedinUserId;
                    cert.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                    cert.ModifiedDate = DateTime.UtcNow;
                    cert.IsActive = true;
                    using var connection = _context.CreateConnection();
                    connection.Open();
                    await connection.ExecuteAsync(query, cert);
                    connection.Close();

                    await SendProfileUpdateEmail(cert.EmployeeCode.ToString());


                    return StatusCode(200, new
                    {
                        StatusCode = 200,
                        //Message = "Site created successfully!",
                        //Data = new { Id = productId }
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500
                    });

                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
            //return Request.CreateResponse(HttpStatusCode.OK, new { StickerAlreadyExist = StickerAlreadyExist });

        }

        [HttpPost("UpsertEmployeeBio")]
        public async Task<IActionResult> UpsertEmployeeBio(EmployeeBio employee)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                if (employee != null)
                {
                    if (employee.Id == null)
                    {
                        var InsertQuery = @"INSERT INTO EmployeeBio 
                                            (
                                               EmployeeCode
                                              ,About
                                              ,Hobbies
                                              ,FavoriteBooks
                                              ,MusicPreference
                                              ,Sports
                                              ,CreatedById
                                              ,CreatedBy
                                              ,CreatedDate
                                              ,ModifiedById
                                              ,ModifiedBy
                                              ,ModifiedDate
                                              ,IsActive
                                            )
                                            VALUES 
                                            (
                                               @EmployeeCode
                                              ,@About
                                              ,@Hobbies
                                              ,@FavoriteBooks
                                              ,@MusicPreference
                                              ,@Sports
                                              ,@CreatedById
                                              ,@CreatedBy
                                              ,@CreatedDate
                                              ,@ModifiedById
                                              ,@ModifiedBy
                                              ,@ModifiedDate
                                              ,@IsActive
                                            );";

                        employee.CreatedById = loggedinUserId;
                        employee.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        employee.CreatedDate = DateTime.UtcNow;
                        employee.ModifiedById = loggedinUserId;
                        employee.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        employee.ModifiedDate = DateTime.UtcNow;
                        employee.IsActive = true;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(InsertQuery, employee);
                        connection.Close();

                        await SendProfileUpdateEmail(employee.EmployeeCode.ToString());

                    }
                    else
                    {
                        var UpdateQuery = @"UPDATE EmployeeBio
                                            SET 
                                               About = @About 
                                              ,Hobbies = @Hobbies 
                                              ,FavoriteBooks = @FavoriteBooks 
                                              ,MusicPreference = @MusicPreference 
                                              ,Sports = @Sports 
                                              ,ModifiedById = @ModifiedById 
                                              ,ModifiedBy = @ModifiedBy 
                                              ,ModifiedDate = @ModifiedDate 
                                               WHERE EmployeeCode = @EmployeeCode";

                        employee.ModifiedById = loggedinUserId;
                        employee.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        employee.ModifiedDate = DateTime.UtcNow;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(UpdateQuery, employee);
                        connection.Close();

                        await SendProfileUpdateEmail(employee.EmployeeCode.ToString());

                    }


                }

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    //Message = "Site created successfully!",
                    //Data = new { Id = productId }
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
            //return Request.CreateResponse(HttpStatusCode.OK, new { StickerAlreadyExist = StickerAlreadyExist });

        }

        [HttpPost("UpsertSupportingDocuments")]
        public async Task<IActionResult> UpsertSupportingDocuments(SupportingDocument supportingDoc)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                if (supportingDoc != null)
                {
                    if (supportingDoc.Id == null)
                    {
                        var InsertQuery = @"INSERT INTO SupportingDocuments
                                            (
                                                EmployeeCode
                                               ,DocumentName
                                               ,DocumentFileName
                                               ,DocumentUrl
                                               ,CreatedById
                                               ,CreatedBy
                                               ,CreatedDate
                                               ,ModifiedById
                                               ,ModifiedBy
                                               ,ModifiedDate
                                               ,IsActive
                                               ,IsDeletedApproved
                                            )
                                            VALUES (
                                                @EmployeeCode
                                               ,@DocumentName
                                               ,@DocumentFileName
                                               ,@DocumentUrl
                                               ,@CreatedById
                                               ,@CreatedBy
                                               ,@CreatedDate
                                               ,@ModifiedById
                                               ,@ModifiedBy
                                               ,@ModifiedDate
                                               ,@IsActive
                                               ,@IsDeletedApproved
                                            );";

                        supportingDoc.CreatedById = loggedinUserId;
                        supportingDoc.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        supportingDoc.CreatedDate = DateTime.UtcNow;
                        supportingDoc.ModifiedById = loggedinUserId;
                        supportingDoc.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        supportingDoc.ModifiedDate = DateTime.UtcNow;
                        supportingDoc.IsActive = true;
                        supportingDoc.IsDeletedApproved = false;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(InsertQuery, supportingDoc);
                        connection.Close();

                        await SendProfileUpdateEmail(supportingDoc.EmployeeCode.ToString());

                        return StatusCode(200, new
                        {
                            StatusCode = 200
                        });
                    }
                    else
                    {
                        var UpdateQuery = @"UPDATE SupportingDocuments
                                            SET
                                                EmployeeCode = @EmployeeCode
                                               ,DocumentName = @DocumentName
                                               ,DocumentFileName = @DocumentFileName
                                               ,DocumentUrl = @DocumentUrl
                                               ,ModifiedById = @ModifiedById
                                               ,ModifiedBy = @ModifiedBy
                                               ,ModifiedDate = @ModifiedDate
                                               ,IsActive = @IsActive
                                            WHERE
                                                Id = @Id;";

                        supportingDoc.ModifiedById = loggedinUserId;
                        supportingDoc.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        supportingDoc.ModifiedDate = DateTime.UtcNow;
                        supportingDoc.IsActive = true;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(UpdateQuery, supportingDoc);
                        connection.Close();

                        await SendProfileUpdateEmail(supportingDoc.EmployeeCode.ToString());

                        return StatusCode(200, new
                        {
                            StatusCode = 200,
                        });
                    }
                }
                else
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500
                    });
                }

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
            //return Request.CreateResponse(HttpStatusCode.OK, new { StickerAlreadyExist = StickerAlreadyExist });

        }

        [HttpGet("GetEmployeeById")]
        public async Task<ActionResult> GetEmployeeById(string Id)
        {
            try
            {
                var employeequery = "Select * from Employee Where Id = @Id";
                var paramas = new { Id = Id };
                using var connection = _context.CreateConnection();
                connection.Open();
                var employeedata = await connection.QueryFirstOrDefaultAsync<Employee>(employeequery, paramas);
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    Employee = employeedata
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }

        [HttpGet("GetEmployeeByEmpCode")]
        public async Task<ActionResult> GetEmployeeByEmpCode(int? EmployeeCode)
        {
            try
            {
                var employeequery = @"Select E.*,U.UserRoles as UserRoles,R.RoleName from Employee E 
                                      LEFT Join Users U On U.EmployeeCode = E.EmployeeCode
                                      LEFT Join Roles R ON R.RoleId = U.UserRoles
                                      Where E.EmployeeCode = @EmployeeCode";
                var paramas = new { EmployeeCode = EmployeeCode };
                using var connection = _context.CreateConnection();
                connection.Open();
                var employeedata = await connection.QueryFirstOrDefaultAsync<Employee>(employeequery, paramas);
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    Employee = employeedata
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }

        [HttpGet("GetEmployeeEmergencyConatcts")]
        public async Task<ActionResult> GetEmployeeEmergencyConatcts(int? EmployeeCode)
        {
            try
            {
                var employeequery = "Select * from EmployeeEmergencyContacts Where EmployeeCode = @EmployeeCode and IsActive = @IsActive";
                var paramas = new { EmployeeCode = EmployeeCode, IsActive = true };
                using var connection = _context.CreateConnection();
                connection.Open();
                var contactsdata = await connection.QueryAsync<EmployeeEmergencyContacts>(employeequery, paramas);
                var ContactsList = contactsdata.ToList<EmployeeEmergencyContacts>();

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    ContactsList = ContactsList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }

        [HttpGet("GetEmployeeEducationDetails")]
        public async Task<ActionResult> GetEmployeeEducationDetails(int? EmployeeCode)
        {
            try
            {
                var employeequery = "Select * from EmployeeEducation Where EmployeeCode = @EmployeeCode and IsActive = @IsActive";
                var paramas = new { EmployeeCode = EmployeeCode, IsActive = true };
                using var connection = _context.CreateConnection();
                connection.Open();
                var educationdata = await connection.QueryAsync<EducationDetails>(employeequery, paramas);
                var EducationDetailsList = educationdata.ToList<EducationDetails>();

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    EducationDetails = EducationDetailsList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }

        [HttpGet("GetEquipmentByEmpCode")]
        public async Task<ActionResult> GetEquipmentByEmpCode(int? EmployeeCode)
        {
            try
            {
                var equipmentquery = "Select * from EmployeeEquipment Where EmployeeCode = @EmployeeCode and IsActive = 1";
                var paramas = new { EmployeeCode = EmployeeCode };
                using var connection = _context.CreateConnection();
                connection.Open();
                var equipmentdata = await connection.QueryAsync<EmployeeEquipment>(equipmentquery, paramas);
                var EquipmentsList = equipmentdata.ToList<EmployeeEquipment>();

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    Equipments = EquipmentsList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }

        [HttpGet("GetCertByEmpCode")]
        public async Task<ActionResult> GetCertByEmpCode(int? EmployeeCode)
        {
            try
            {
                var equipmentquery = "Select * from EmployeeLicenseCertification Where EmployeeCode = @EmployeeCode and IsActive = 1";
                var paramas = new { EmployeeCode = EmployeeCode };
                using var connection = _context.CreateConnection();
                connection.Open();
                var certdata = await connection.QueryAsync<CerificateLicense>(equipmentquery, paramas);
                var CertList = certdata.ToList<CerificateLicense>();

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    CertLicenses = CertList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }

        [HttpGet("GetEmployeeBioByEmpCode")]
        public async Task<ActionResult> GetEmployeeBioByEmpCode(int? EmployeeCode)
        {
            try
            {
                var equipmentquery = "Select * from EmployeeBio Where EmployeeCode = @EmployeeCode and IsActive = 1";
                var paramas = new { EmployeeCode = EmployeeCode };
                using var connection = _context.CreateConnection();
                connection.Open();
                var bio = await connection.QueryAsync<EmployeeBio>(equipmentquery, paramas);

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    EmployeeBio = bio.FirstOrDefault()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }

        [HttpGet("GetSupportingDocsByEmpCode")]
        public async Task<ActionResult> GetSupportingDocsByEmpCode(int? EmployeeCode)
        {
            try
            {
                var query = "Select * from SupportingDocuments Where EmployeeCode = @EmployeeCode and IsActive = 1";
                var paramas = new { EmployeeCode = EmployeeCode };
                using var connection = _context.CreateConnection();
                connection.Open();
                var docsdata = await connection.QueryAsync<SupportingDocument>(query, paramas);
                var DocsList = docsdata.ToList<SupportingDocument>();

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    SupportingDocs = DocsList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }

        [HttpPost("DeleteEmployeeById")]
        public async Task<ActionResult> DeleteEmployeeById(string Id, string TerminationDismissalDate, int TerminationDismissalReasonId, string TerminationDismissalReason, string? TerminationDismissalComment, string TerminationDismissalType)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var employeequery = @"Update Employee SET
                                   TerminationDismissalDate = @TerminationDismissalDate
                                  ,TerminationDismissalReasonId = @TerminationDismissalReasonId
                                  ,TerminationDismissalReason = @TerminationDismissalReason
                                  ,TerminationDismissalComment = @TerminationDismissalComment
                                  ,TerminationDismissalType = @TerminationDismissalType
                                  ,ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,Status = @Status 
                                   Where Id = @Id";


                if (!string.IsNullOrEmpty(TerminationDismissalDate))
                {
                    TerminationDismissalDate = DateTime.ParseExact(TerminationDismissalDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                }
                var paramas = new
                {
                    Id = Id,
                    TerminationDismissalDate = TerminationDismissalDate,
                    TerminationDismissalReasonId = TerminationDismissalReasonId,
                    TerminationDismissalReason = TerminationDismissalReason,
                    TerminationDismissalComment = TerminationDismissalComment,
                    TerminationDismissalType = TerminationDismissalType,
                    ModifiedById = loggedinUserId,
                    ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName,
                    ModifiedDate = DateTime.UtcNow,
                    Status = 3
                };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(employeequery, paramas);
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }


        [HttpPost("DeleteEmergencyConatctById")]
        public async Task<ActionResult> DeleteEmergencyConatctById(string Id)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var employeequery = @"Update EmployeeEmergencyContacts SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                   Where Id = @Id";
                var paramas = new { Id = Id, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(employeequery, paramas);
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }


        [HttpPost("DeleteEducationDetailById")]
        public async Task<ActionResult> DeleteEducationDetailById(string Id)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var employeequery = @"Update EmployeeEducation SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                   Where Id = @Id";
                var paramas = new { Id = Id, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(employeequery, paramas);
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }

        [HttpPost("DeleteEquipmentById")]
        public async Task<ActionResult> DeleteEquipmentById(string Id)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var query = @"Update EmployeeEquipment SET
                                   Status = @Status 
                                  ,ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                   Where Id = @Id";
                var paramas = new { Id = Id, Status = 4, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(query, paramas);
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }


        [HttpPost("DeleteSupportingDocumentById")]
        public async Task<ActionResult> DeleteSupportingDocumentById(string Id)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var query = @"Update SupportingDocuments SET
                                   IsActive = @IsActive 
                                  ,ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                   Where Id = @Id";
                var paramas = new { Id = Id, IsActive = false, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(query, paramas);
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }


        [HttpPost("GetEmployeesList")]
        public async Task<IActionResult> GetEmployeesList(EmployeeFilter filters)
        {
            try
            {
                var tuple = await FilterEmployeesListData(filters);
                return Ok(new
                {
                    StatusCode = 200,
                    Employees = tuple.Item1,
                    TotalRecords = tuple.Item2,
                    AllEmployees = tuple.Item3,
                    ActiveEmployees = tuple.Item4,
                    AllContractorCount = tuple.Item5,
                    ActiveContractorCount = tuple.Item6
                });
            }
            catch (Exception ex)
            {

                ExceptionLogger.LogException(ex);
                throw ex;
            }
        }


        private async Task<Tuple<List<Employee>, int, int, int,int,int>> FilterEmployeesListData(EmployeeFilter search)
        {
            //var timezoneoffset = Convert.ToInt32(HttpContext.Current.Request.Cookies["timeZoneOffsetCookie"].Value);
            try
            {
                var sqlParams = new List<SqlParameter>();
                var sqlParams1 = new List<SqlParameter>();

                var whereClause = $" Where (E.Status = 1 OR E.Status = 2)";
                var SitesWhereClause = "";
                if (search.FormerEmployee == 1)
                {
                    whereClause = $" Where (E.Status = 1 OR E.Status = 2 OR E.Status = 3)";
                }

                if (!string.IsNullOrEmpty(search.FirstName))
                {
                    whereClause += "\n AND E.FirstName like '%'+@FirstName+'%'";
                    sqlParams.Add(new SqlParameter("@FirstName", search.FirstName));
                    sqlParams1.Add(new SqlParameter("@FirstName", search.FirstName));
                }
                if (!string.IsNullOrEmpty(search.LastName))
                {
                    whereClause += "\n AND E.LastName like '%'+@LastName+'%'";
                    sqlParams.Add(new SqlParameter("@LastName", search.LastName));
                    sqlParams1.Add(new SqlParameter("@LastName", search.LastName));
                }
                if (search.DepartmentId != null)
                {
                    whereClause += "\n AND E.DepartmentId = @DepartmentId";
                    sqlParams.Add(new SqlParameter("@DepartmentId", search.DepartmentId));
                    sqlParams1.Add(new SqlParameter("@DepartmentId", search.DepartmentId));

                }
                if (search.SiteId != null && search.SiteId.Count > 0)
                {
                    SitesWhereClause += "\n AND j.id in (" + string.Join(",", search.SiteId) + ") ";
                    //sqlParams.Add(new SqlParameter("@SiteId", search.SiteId));
                    //sqlParams1.Add(new SqlParameter("@SiteId", search.SiteId));

                }
                if (search.PositionId != null)
                {
                    whereClause += "\n AND E.PositionId = @PositionId";
                    sqlParams.Add(new SqlParameter("@PositionId", search.PositionId));
                    sqlParams1.Add(new SqlParameter("@PositionId", search.PositionId));
                }
                if (search.EmploymentStatusId != null)
                {
                    whereClause += "\n AND E.EmploymentStatusId = @EmploymentStatusId";
                    sqlParams.Add(new SqlParameter("@EmploymentStatusId", search.EmploymentStatusId));
                    sqlParams1.Add(new SqlParameter("@EmploymentStatusId", search.EmploymentStatusId));
                }


                var orderByClause = " ";

                if (string.IsNullOrEmpty(search.SortCol))
                {
                    orderByClause = "\n ORDER BY E.ModifiedDate DESC";

                }
                else
                {
                    if (search.sSortDir_0.Equals("asc"))
                    {
                        if (search.SortCol.Equals("SiteName"))
                        {
                            orderByClause = "\n ORDER BY E.SiteName asc";
                        }
                        else if (search.SortCol.Equals("TimeZone"))
                        {
                            orderByClause = "\n ORDER BY E.TimeZone asc";

                        }
                        else if (search.SortCol.Equals("CountryName"))
                        {
                            orderByClause = "\n ORDER BY E.CountryName asc";
                        }
                    }
                    if (search.sSortDir_0.Equals("desc"))
                    {

                        if (search.SortCol.Equals("SiteName"))
                        {
                            orderByClause = "\n ORDER BY E.SiteName desc";
                        }
                        else if (search.SortCol.Equals("TimeZone"))
                        {
                            orderByClause = "\n ORDER BY E.TimeZone desc";

                        }
                        else if (search.SortCol.Equals("CountryName"))
                        {
                            orderByClause = "\n ORDER BY E.CountryName desc";
                        }
                    }

                }

                var joins = @" LEFT Join Users U On U.EmployeeCode = E.EmployeeCode
                               LEFT Join Roles R ON R.RoleId = U.UserRoles";

                var sqlForCount = @"WITH FilteredEmployees AS (
                                        SELECT 
                                            E.*,
                                            R.RoleName,
                                            j.[id] AS ParsedSiteId
                                        FROM dbo.Employee E
                                        LEFT JOIN Users U ON U.EmployeeCode = E.EmployeeCode
                                        LEFT JOIN Roles R ON R.RoleId = U.UserRoles
                                        CROSS APPLY OPENJSON(E.SiteName)
                                        WITH (
                                            id INT,
                                            name NVARCHAR(200)
                                        ) AS j
                                        WHERE (E.Status = 1 OR E.Status = 2) " + SitesWhereClause
                                   + @" )
                                    SELECT 
                                         Count(E.Id)
                                    FROM FilteredEmployees E "
                                   + joins
                                   + whereClause;


                using var connection = _context.CreateConnection();
                connection.Open();
                var totalCount = await connection.QuerySingleAsync<int>(sqlForCount, search);

                var sqlForAllCount = @"SELECT COUNT(E.Id) FROM dbo.Employee E WHERE E.EmploymentStatus IS NULL OR Lower(E.EmploymentStatus) != 'contractor'";


                var AllCount = await connection.QuerySingleAsync<int>(sqlForAllCount, search);


                var sqlForActiveCount = @"SELECT COUNT(E.Id) FROM dbo.Employee E Where E.Status = 1 AND (E.EmploymentStatus IS NULL OR Lower(E.EmploymentStatus) != 'contractor')";


                var ActiveCount = await connection.QuerySingleAsync<int>(sqlForActiveCount, search);


                var sqlForAllContarctorCount = @"SELECT COUNT(E.Id) FROM dbo.Employee E where Lower(E.EmploymentStatus) = 'contractor'";


                var AllContractorCount = await connection.QuerySingleAsync<int>(sqlForAllContarctorCount, search);


                var sqlForActiveContractorCount = @"SELECT COUNT(E.Id) FROM dbo.Employee E Where E.Status = 1 AND Lower(E.EmploymentStatus) = 'contractor'";


                var ActiveContractorCount = await connection.QuerySingleAsync<int>(sqlForActiveContractorCount, search);
                //connection.Close();

                var sql = @"WITH FilteredEmployees AS (
                                SELECT 
                                    E.*,
                                    R.RoleName,
                                    j.[id] AS ParsedSiteId
                                FROM dbo.Employee E
                                LEFT JOIN Users U ON U.EmployeeCode = E.EmployeeCode
                                LEFT JOIN Roles R ON R.RoleId = U.UserRoles
                                CROSS APPLY OPENJSON(E.SiteName)
                                WITH (
                                    id INT,
                                    name NVARCHAR(200)
                                ) AS j
                                WHERE (E.Status = 1 OR E.Status = 2) " + SitesWhereClause
                           + @" )
                            SELECT 
                                 Distinct(E.Id)
                                ,E.ProfilePhotoUrl
                                ,E.EmployeeCode
                                ,E.FirstName
                                ,E.LastName
                                ,E.HiringDate
                                ,E.DOB
                                ,E.Gender
                                ,E.Email
                                ,E.PhoneNumber
                                ,E.City
                                ,E.PostalCode
                                ,E.DepartmentId
                                ,E.DepartmentName
                                ,E.SiteId
                                ,E.SiteName
                                ,E.Address
                                ,E.PositionId
                                ,E.PositionName
                                ,E.MaritalStatus
                                ,E.ManagerId
                                ,E.ManagerName
                                ,E.AlternativeEmail
                                ,E.ProbationDateStart
                                ,E.ProbationDateEnd
                                ,E.EmploymentStatusId
                                ,E.EmploymentStatus
                                ,E.EmploymentLevelId
                                ,E.EmploymentLevel
                                ,E.CreatedById
                                ,E.CreatedBy
                                ,E.CreatedDate
                                ,E.ModifiedById
                                ,E.ModifiedBy
                                ,E.ModifiedDate
                                ,E.Status
                                ,E.RoleName
                            FROM FilteredEmployees E"
                            + joins
                            + whereClause
                            + orderByClause
                            + "\n OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                search.Offset = search.PageNumber * search.PageSize;
                var data = await connection.QueryAsync<Employee>(sql, search);
                var EmployeeList = data.ToList<Employee>();
                //var data = db.Database.SqlQuery<Sites>(sql, sqlParams.ToArray()).ToList();
                return new Tuple<List<Employee>, int, int, int,int,int>(EmployeeList, totalCount, AllCount, ActiveCount,AllContractorCount,ActiveContractorCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<Tuple<List<Employee>, int, int, int>> FilterFormerEmployeesListData(EmployeeFilter search)
        {
            //var timezoneoffset = Convert.ToInt32(HttpContext.Current.Request.Cookies["timeZoneOffsetCookie"].Value);
            try
            {
                var sqlParams = new List<SqlParameter>();
                var sqlParams1 = new List<SqlParameter>();

                var whereClause = $" Where (E.Status = 3)";
                var SitesWhereClause = "";

                if (!string.IsNullOrEmpty(search.FirstName))
                {
                    whereClause += "\n AND E.FirstName like '%'+@FirstName+'%'";
                    sqlParams.Add(new SqlParameter("@FirstName", search.FirstName));
                    sqlParams1.Add(new SqlParameter("@FirstName", search.FirstName));
                }
                if (!string.IsNullOrEmpty(search.LastName))
                {
                    whereClause += "\n AND E.LastName like '%'+@LastName+'%'";
                    sqlParams.Add(new SqlParameter("@LastName", search.LastName));
                    sqlParams1.Add(new SqlParameter("@LastName", search.LastName));
                }
                if (search.DepartmentId != null)
                {
                    whereClause += "\n AND E.DepartmentId = @DepartmentId";
                    sqlParams.Add(new SqlParameter("@DepartmentId", search.DepartmentId));
                    sqlParams1.Add(new SqlParameter("@DepartmentId", search.DepartmentId));

                }
                if (search.SiteId != null && search.SiteId.Count > 0)
                {
                    SitesWhereClause += "\n AND j.id in (" + string.Join(",", search.SiteId) + ") ";
                    //sqlParams.Add(new SqlParameter("@SiteId", search.SiteId));
                    //sqlParams1.Add(new SqlParameter("@SiteId", search.SiteId));

                }
                if (search.PositionId != null)
                {
                    whereClause += "\n AND E.PositionId = @PositionId";
                    sqlParams.Add(new SqlParameter("@PositionId", search.PositionId));
                    sqlParams1.Add(new SqlParameter("@PositionId", search.PositionId));
                }


                var orderByClause = " ";

                if (string.IsNullOrEmpty(search.SortCol))
                {
                    orderByClause = "\n ORDER BY E.ModifiedDate DESC";

                }
                else
                {
                    if (search.sSortDir_0.Equals("asc"))
                    {
                        if (search.SortCol.Equals("SiteName"))
                        {
                            orderByClause = "\n ORDER BY E.SiteName asc";
                        }
                        else if (search.SortCol.Equals("TimeZone"))
                        {
                            orderByClause = "\n ORDER BY E.TimeZone asc";

                        }
                        else if (search.SortCol.Equals("CountryName"))
                        {
                            orderByClause = "\n ORDER BY E.CountryName asc";
                        }
                    }
                    if (search.sSortDir_0.Equals("desc"))
                    {

                        if (search.SortCol.Equals("SiteName"))
                        {
                            orderByClause = "\n ORDER BY E.SiteName desc";
                        }
                        else if (search.SortCol.Equals("TimeZone"))
                        {
                            orderByClause = "\n ORDER BY E.TimeZone desc";

                        }
                        else if (search.SortCol.Equals("CountryName"))
                        {
                            orderByClause = "\n ORDER BY E.CountryName desc";
                        }
                    }

                }

                var joins = @"";

                var sqlForCount = @"WITH FilteredEmployees AS (
                                        SELECT 
                                            E.*,
                                            R.RoleName,
                                            j.[id] AS ParsedSiteId
                                        FROM dbo.Employee E
                                        LEFT JOIN Users U ON U.EmployeeCode = E.EmployeeCode
                                        LEFT JOIN Roles R ON R.RoleId = U.UserRoles
                                        CROSS APPLY OPENJSON(E.SiteName)
                                        WITH (
                                            id INT,
                                            name NVARCHAR(200)
                                        ) AS j
                                        WHERE (E.Status = 1 OR E.Status = 2) " + SitesWhereClause
                                   + @" )
                                    SELECT 
                                         Count(E.Id)
                                    FROM FilteredEmployees E "
                                   + joins
                                   + whereClause;



                using var connection = _context.CreateConnection();
                connection.Open();
                var totalCount = await connection.QuerySingleAsync<int>(sqlForCount, search);

                var sqlForAllCount = @"SELECT COUNT(E.Id) FROM dbo.Employee E";


                var AllCount = await connection.QuerySingleAsync<int>(sqlForAllCount, search);

                var sqlForActiveCount = @"SELECT COUNT(E.Id) FROM dbo.Employee E Where E.Status = 1 OR E.Status = 2";


                var ActiveCount = await connection.QuerySingleAsync<int>(sqlForActiveCount, search);
                //connection.Close();
                var sql = @"WITH FilteredEmployees AS (
                                SELECT 
                                    E.*,
                                    R.RoleName,
                                    j.[id] AS ParsedSiteId
                                FROM dbo.Employee E
                                LEFT JOIN Users U ON U.EmployeeCode = E.EmployeeCode
                                LEFT JOIN Roles R ON R.RoleId = U.UserRoles
                                CROSS APPLY OPENJSON(E.SiteName)
                                WITH (
                                    id INT,
                                    name NVARCHAR(200)
                                ) AS j
                                WHERE (E.Status = 1 OR E.Status = 2) " + SitesWhereClause
                           + @" )
                            SELECT 
                                 E.Id
                                ,E.ProfilePhotoUrl
                                ,E.EmployeeCode
                                ,E.FirstName
                                ,E.LastName
                                ,E.HiringDate
                                ,E.DOB
                                ,E.Gender
                                ,E.Email
                                ,E.PhoneNumber
                                ,E.City
                                ,E.PostalCode
                                ,E.DepartmentId
                                ,E.DepartmentName
                                ,E.SiteId
                                ,E.SiteName
                                ,E.Address
                                ,E.PositionId
                                ,E.PositionName
                                ,E.MaritalStatus
                                ,E.ManagerId
                                ,E.ManagerName
                                ,E.AlternativeEmail
                                ,E.ProbationDateStart
                                ,E.ProbationDateEnd
                                ,E.EmploymentStatusId
                                ,E.EmploymentStatus
                                ,E.EmploymentLevelId
                                ,E.EmploymentLevel
                                ,E.CreatedById
                                ,E.CreatedBy
                                ,E.CreatedDate
                                ,E.ModifiedById
                                ,E.ModifiedBy
                                ,E.ModifiedDate
                                ,E.Status
                                ,E.RoleName
                            FROM FilteredEmployees E"
                            + joins
                            + whereClause
                            + orderByClause
                            + "\n OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                search.Offset = search.PageNumber * search.PageSize;
                var data = await connection.QueryAsync<Employee>(sql, search);
                var EmployeeList = data.ToList<Employee>();
                //var data = db.Database.SqlQuery<Sites>(sql, sqlParams.ToArray()).ToList();
                return new Tuple<List<Employee>, int, int, int>(EmployeeList, totalCount, AllCount, ActiveCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(ImageUploadModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Base64) || string.IsNullOrEmpty(model.FileName))
                    return StatusCode(500, new { StatusCode = HttpStatusCode.InternalServerError, imageUrl = "" });

                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "profile-photos");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);



                string imagePath = Path.Combine(uploadsFolder, model.FileName);

                // Remove base64 prefix (e.g., "data:image/png;base64,")
                string base64Data = Regex.Replace(model.Base64, @"^data:image\/[a-zA-Z]+;base64,", "");
                byte[] imageBytes = Convert.FromBase64String(base64Data);

                await System.IO.File.WriteAllBytesAsync(imagePath, imageBytes);


                var relativePath = "/profile-photos/" + model.FileName;

                return StatusCode(200, new { StatusCode = HttpStatusCode.OK, imageUrl = relativePath });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }

        [HttpPost("UploadCertImage")]
        public async Task<IActionResult> UploadCertImage(ImageUploadModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Base64) || string.IsNullOrEmpty(model.FileName))
                    return StatusCode(500, new { StatusCode = HttpStatusCode.InternalServerError, imageUrl = "" });

                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "cert-license");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);



                string imagePath = Path.Combine(uploadsFolder, model.FileName);

                // Remove base64 prefix (e.g., "data:image/png;base64,")
                string base64Data = Regex.Replace(model.Base64, @"^data:image\/[a-zA-Z]+;base64,", "");
                byte[] imageBytes = Convert.FromBase64String(base64Data);

                await System.IO.File.WriteAllBytesAsync(imagePath, imageBytes);


                var relativePath = "/cert-license/" + model.FileName;

                return StatusCode(200, new { StatusCode = HttpStatusCode.OK, imageUrl = relativePath });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }

        [HttpPost("UploadDocuments")]
        public async Task<IActionResult> UploadDocuments(ImageUploadModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Base64) || string.IsNullOrEmpty(model.FileName))
                    return StatusCode(500, new { StatusCode = HttpStatusCode.InternalServerError, imageUrl = "" });

                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "emp-documents");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Extract extension from uploaded filename
                string extension = Path.GetExtension(model.FileName);

                // Generate a new unique filename
                model.FileName = $"{Guid.NewGuid()}{extension}";


                string filePath = Path.Combine(uploadsFolder, model.FileName);

                // If Base64 has a prefix (like data:image/png;base64,), remove it safely
                string base64Data = model.Base64;

                var dataPrefixMatch = Regex.Match(base64Data, @"^data:(.+?);base64,");
                if (dataPrefixMatch.Success)
                {
                    base64Data = base64Data.Substring(dataPrefixMatch.Value.Length);
                }

                byte[] fileBytes = Convert.FromBase64String(base64Data);
                await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);

                var relativePath = "/emp-documents/" + model.FileName;

                return StatusCode(200, new { StatusCode = HttpStatusCode.OK, fileUrl = relativePath, fileName = model.FileName });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }

        [HttpGet("GetEmployeeBySiteDepartment")]
        public async Task<IActionResult> GetEmployeeBySiteDepartment(int? siteId, int? departmentId)
        {
            try
            {
                var query = @"
                SELECT *
                FROM Employee 
                WHERE (@SiteId IS NULL OR SiteId = @SiteId)
                AND (@DepartmentId IS NULL OR DepartmentId = @DepartmentId)";

                var paramas = new { SiteId = siteId, DepartmentId = departmentId };
                using var connection = _context.CreateConnection();
                connection.Open();
                var Employees = await connection.QueryAsync<Employee>(query, paramas);
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    Employee = Employees
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }

        [HttpGet("GetEmployeeNotifications")]
        public async Task<IActionResult> GetEmployeeNotifications(int? EmployeeCode)
        {
            List<Notifications> notif = new List<Notifications>();
            try
            {
                var Query = @"Select * from Notifications Where EmployeeCode = @EmployeeCode and IsActive = 1 ORDER BY CreatedDate DESC";

                var user = new { EmployeeCode = EmployeeCode };
                using var connection = _context.CreateConnection();
                connection.Open();
                var notifications = connection.Query<Notifications>(Query, user);
                connection.Close();
                notif = notifications.ToList<Notifications>();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    Notifications = notif
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    Notifications = notif
                });

            }

        }

        [HttpPost("MarkNotificationAsRead")]
        public async Task<IActionResult> MarkNotificationAsRead(int? Id)
        {
            try
            {
                var Query = @"Update Notifications Set IsActive = 0 Where Id = @Id";

                var user = new { Id = Id };
                using var connection = _context.CreateConnection();
                connection.Open();
                var notifications = connection.ExecuteAsync(Query, user);
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);

                return StatusCode(200, new
                {
                    StatusCode = 200
                });

            }

        }

        [HttpPost("ExportActiveEmployees")]
        public async Task<IActionResult> ExportActiveEmployees(EmployeeFilter filters)
        {
            try
            {
                filters.PageSize = 10000;
                filters.PageNumber = 0;
                DataTableResponce dtr = new DataTableResponce();
                var tuple = await FilterEmployeesListData(filters);

                if (tuple.Item2 > 0)
                {
                    // Creating Excel workbook
                    using var workbook = new XLWorkbook();
                    var worksheet = workbook.Worksheets.Add("Active Employees");
                    var AttendanceEmployeeName = tuple.Item1.Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();

                    int headerRow = 1; // header row index

                    // Adding Header
                    var headers = new string[]
                    {
                        "EmployeeCode", "FirstName", "LastName", "Email","D.O.B", "MobilePhone", "On-Boarding Date", "Position",
                        "HiringDate", "Site", "Department", "WorkStatus", "DirectManager","Role","SIN","Date of Acceptance",
                        "Current Immigration Status","Are you currently eligible to work in Canada?","Will you require sponsorship to continue working in Canada?",
                        "Other"
                    };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = worksheet.Cell(headerRow, i + 1);
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Fill.BackgroundColor = XLColor.LightGray; // Optional: Header Background Color
                    }


                    // Inserting Data starting from row 4
                    int startDataRow = headerRow + 1;
                    // Inserting Data
                    for (int i = 0; i < tuple.Item1.Count; i++)
                    {
                        int row = startDataRow + i;
                        var record = tuple.Item1[i];

                        var sites = JsonConvert.DeserializeObject<List<Site>>(record.SiteName);
                        List<string> names = new List<string>();
                        foreach (var site in sites)
                        {
                            names.Add(site.name);
                        }
                        string siteNames = string.Join(", ", names);

                        var departments = JsonConvert.DeserializeObject<List<Site>>(record.DepartmentName);
                        List<string> deptnames = new List<string>();
                        foreach (var dept in departments)
                        {
                            names.Add(dept.name);
                        }
                        string departmentNames = string.Join(", ", deptnames);

                        worksheet.Cell(row, 1).Value = record.EmployeeCode;
                        worksheet.Cell(row, 2).Value = record.FirstName;
                        worksheet.Cell(row, 3).Value = record.LastName;
                        worksheet.Cell(row, 4).Value = record.Email;
                        worksheet.Cell(row, 5).Value = record.DOB;
                        worksheet.Cell(row, 6).Value = record.PhoneNumber;
                        worksheet.Cell(row, 7).Value = (!string.IsNullOrEmpty(record.OnBoardingDate) && !string.IsNullOrEmpty(record.OnBoardingDate)) ? DateTime.Parse(record.OnBoardingDate).ToString("MM/dd/yyyy") : "";
                        worksheet.Cell(row, 8).Value = record.PositionName;
                        worksheet.Cell(row, 9).Value = (!string.IsNullOrEmpty(record.HiringDate) && !string.IsNullOrEmpty(record.HiringDate)) ? DateTime.Parse(record.HiringDate).ToString("MM/dd/yyyy") : "";
                        worksheet.Cell(row, 10).Value = siteNames;
                        worksheet.Cell(row, 11).Value = departmentNames;
                        worksheet.Cell(row, 12).Value = record.EmploymentStatus;
                        worksheet.Cell(row, 13).Value = record.ManagerName;
                        worksheet.Cell(row, 14).Value = record.RoleName;
                        worksheet.Cell(row, 15).Value = record.SinNo;
                        worksheet.Cell(row, 16).Value = (!string.IsNullOrEmpty(record.AcceptanceDate) && !string.IsNullOrEmpty(record.AcceptanceDate)) ? DateTime.Parse(record.AcceptanceDate).ToString("MM/dd/yyyy") : "";
                        worksheet.Cell(row, 17).Value = record.ImmigrationStatus;
                        worksheet.Cell(row, 18).Value = record.WorkEligibility;
                        worksheet.Cell(row, 19).Value = record.SponsorShip;
                        worksheet.Cell(row, 20).Value = record.Other;

                        // Applying Borders
                        for (int col = 1; col <= headers.Length; col++)
                        {
                            worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                    }

                    // Auto adjusting columns
                    worksheet.Columns().AdjustToContents();
                    // Saving to MemoryStream
                    using var stream = new MemoryStream();
                    workbook.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin);


                    string FileName = "Active_Employees_" + Guid.NewGuid() + ".xlsx";

                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    else
                    {
                        // Deleting all existing Excel files in the directory
                        foreach (string file in Directory.GetFiles(folderPath, "*.xlsx"))
                        {
                            System.IO.File.Delete(file);
                        }
                    }

                    string filePath = Path.Combine(folderPath, FileName);

                    //Saving Excel file to the project directory
                    await System.IO.File.WriteAllBytesAsync(filePath, stream.ToArray());

                    //Generating public download URL
                    string fileUrl = $"{Request.Scheme}://{Request.Host}/exports/{FileName}";

                    return Ok(new
                    {
                        StatusCode = 200,
                        FileName = FileName,
                        DownloadUrl = fileUrl
                    });
                }
                else
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        FileName = string.Empty,
                        DownloadUrl = string.Empty
                    });
                }

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }

        }


        [HttpPost("ExportFormerEmployees")]
        public async Task<IActionResult> ExportFormerEmployees(EmployeeFilter filters)
        {
            try
            {
                filters.PageSize = 10000;
                filters.PageNumber = 0;
                DataTableResponce dtr = new DataTableResponce();
                var tuple = await FilterFormerEmployeesListData(filters);

                if (tuple.Item2 > 0)
                {
                    // Creating Excel workbook
                    using var workbook = new XLWorkbook();
                    var worksheet = workbook.Worksheets.Add("Former Employees");
                    var AttendanceEmployeeName = tuple.Item1.Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();

                    int headerRow = 1; // header row index

                    // Adding Header
                    var headers = new string[]
                    {
                        "EmployeeCode", "FirstName", "LastName", "Email","D.O.B", "MobilePhone", "On-Boarding Date", "Position",
                        "HiringDate", "Site", "Department", "WorkStatus", "DirectManager","Role","SIN","Date of Acceptance",
                        "Current Immigration Status","Are you currently eligible to work in Canada?","Will you require sponsorship to continue working in Canada?",
                        "Other"
                    };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = worksheet.Cell(headerRow, i + 1);
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Fill.BackgroundColor = XLColor.LightGray; // Optional: Header Background Color
                    }

                    // Inserting Data starting from row 4
                    int startDataRow = headerRow + 1;
                    // Inserting Data
                    for (int i = 0; i < tuple.Item1.Count; i++)
                    {
                        int row = startDataRow + i;
                        var record = tuple.Item1[i];
                        var sites = JsonConvert.DeserializeObject<List<Site>>(record.SiteName);
                        List<string> names = new List<string>();
                        foreach (var site in sites)
                        {
                            names.Add(site.name);
                        }
                        string siteNames = string.Join(", ", names);

                        var departments = JsonConvert.DeserializeObject<List<Site>>(record.DepartmentName);
                        List<string> deptnames = new List<string>();
                        foreach (var dept in departments)
                        {
                            names.Add(dept.name);
                        }
                        string departmentNames = string.Join(", ", deptnames);

                        worksheet.Cell(row, 1).Value = record.EmployeeCode;
                        worksheet.Cell(row, 2).Value = record.FirstName;
                        worksheet.Cell(row, 3).Value = record.LastName;
                        worksheet.Cell(row, 4).Value = record.Email;
                        worksheet.Cell(row, 5).Value = record.DOB;
                        worksheet.Cell(row, 6).Value = record.PhoneNumber;
                        worksheet.Cell(row, 7).Value = (!string.IsNullOrEmpty(record.OnBoardingDate) && !string.IsNullOrEmpty(record.OnBoardingDate)) ? DateTime.Parse(record.OnBoardingDate).ToString("MM/dd/yyyy") : "";
                        worksheet.Cell(row, 8).Value = record.PositionName;
                        worksheet.Cell(row, 9).Value = (!string.IsNullOrEmpty(record.HiringDate) && !string.IsNullOrEmpty(record.HiringDate)) ? DateTime.Parse(record.HiringDate).ToString("MM/dd/yyyy") : "";
                        worksheet.Cell(row, 10).Value = siteNames;
                        worksheet.Cell(row, 11).Value = departmentNames;
                        worksheet.Cell(row, 12).Value = record.EmploymentStatus;
                        worksheet.Cell(row, 13).Value = record.ManagerName;
                        worksheet.Cell(row, 14).Value = record.RoleName;
                        worksheet.Cell(row, 15).Value = record.SinNo;
                        worksheet.Cell(row, 16).Value = (!string.IsNullOrEmpty(record.AcceptanceDate) && !string.IsNullOrEmpty(record.AcceptanceDate)) ? DateTime.Parse(record.AcceptanceDate).ToString("MM/dd/yyyy") : "";
                        worksheet.Cell(row, 17).Value = record.ImmigrationStatus;
                        worksheet.Cell(row, 18).Value = record.WorkEligibility;
                        worksheet.Cell(row, 19).Value = record.SponsorShip;
                        worksheet.Cell(row, 20).Value = record.Other;

                        // Applying Borders
                        for (int col = 1; col <= headers.Length; col++)
                        {
                            worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                    }

                    // Auto adjusting columns
                    worksheet.Columns().AdjustToContents();
                    // Saving to MemoryStream
                    using var stream = new MemoryStream();
                    workbook.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin);


                    string FileName = "Former_Employees_" + Guid.NewGuid() + ".xlsx";

                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    else
                    {
                        // Deleting all existing Excel files in the directory
                        foreach (string file in Directory.GetFiles(folderPath, "*.xlsx"))
                        {
                            System.IO.File.Delete(file);
                        }
                    }

                    string filePath = Path.Combine(folderPath, FileName);

                    //Saving Excel file to the project directory
                    await System.IO.File.WriteAllBytesAsync(filePath, stream.ToArray());

                    //Generating public download URL
                    string fileUrl = $"{Request.Scheme}://{Request.Host}/exports/{FileName}";

                    return Ok(new
                    {
                        StatusCode = 200,
                        FileName = FileName,
                        DownloadUrl = fileUrl
                    });
                }
                else
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        FileName = string.Empty,
                        DownloadUrl = string.Empty
                    });
                }

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }

        }

        [HttpPost("ImportEmployees")]
        public async Task<IActionResult> ImportEmployees([FromForm] IFormFile file)
        {
            List<Employee> existingEmployees = new();
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { StatusCode = HttpStatusCode.BadRequest });

                string filePath = Path.GetTempFileName(); // Save file temporarily
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var employees = ReadExcel(filePath); // Read Excel

                foreach (var employee in employees)
                {
                    using var connection = _context.CreateConnection();
                    connection.Open();
                    var employeequery = "Select * from Employee Where Email = @Email";
                    var __params = new { Email = employee.Email };
                    var employeedata = await connection.QueryFirstOrDefaultAsync<Employee>(employeequery, __params);

                    //var _employeequery = "Select * from Employee Where EmployeeCode = @EmployeeCode";
                    //var _params = new { EmployeeCode = employee.EmployeeCode };
                    //var _employeedata = await connection.QueryFirstOrDefaultAsync<Employee>(_employeequery, _params);

                    var ___employeequery = "Select * from Employee Where DOB = @DOB OR EmployeeCode = @EmployeeCode";
                    var ___params = new { EmployeeCode = employee.EmployeeCode, DOB = employee.DOB };
                    var ___employeedata = await connection.QueryFirstOrDefaultAsync<Employee>(___employeequery, ___params);
                    connection.Close();

                    if (employeedata != null)
                    {
                        existingEmployees.Add(employee);
                    }
                    //else if (_employeedata != null)
                    //{
                    //    existingEmployees.Add(employee);
                    //}
                    else if (___employeedata != null)
                    {
                        existingEmployees.Add(employee);
                    }
                    else
                    {

                        var InsertQuery = @"
                                            SELECT @EmployeeCode = ISNULL(MAX(EmployeeCode), 0) + 1 FROM Employee;

                                            INSERT INTO Employee 
                                            (
                                               EmployeeCode
                                              ,FirstName
                                              ,LastName
                                              ,Email
                                              ,DOB
                                              ,PhoneNumber
                                              ,OnBoardingDate
                                              ,DepartmentId
                                              ,DepartmentName
                                              ,SiteName
                                              ,PositionId
                                              ,PositionName
                                              ,ManagerId
                                              ,ManagerName
                                              ,SinNo
                                              ,AcceptanceDate
                                              ,ImmigrationStatus
                                              ,WorkEligibility
                                              ,SponsorShip
                                              ,Other
                                              ,EmploymentStatusId
                                              ,EmploymentStatus
                                              ,CreatedById
                                              ,CreatedBy
                                              ,CreatedDate
                                              ,ModifiedById
                                              ,ModifiedBy
                                              ,ModifiedDate
                                              ,Status
                                            )
                                            VALUES 
                                            (
                                               @EmployeeCode
                                              ,@FirstName
                                              ,@LastName
                                              ,@Email
                                              ,@DOB
                                              ,@PhoneNumber
                                              ,@OnBoardingDate
                                              ,@DepartmentId
                                              ,@DepartmentName
                                              ,@SiteName
                                              ,@PositionId
                                              ,@PositionName
                                              ,@ManagerId
                                              ,@ManagerName
                                              ,@SinNo
                                              ,@AcceptanceDate
                                              ,@ImmigrationStatus
                                              ,@WorkEligibility
                                              ,@SponsorShip
                                              ,@Other
                                              ,@EmploymentStatusId
                                              ,@EmploymentStatus
                                              ,@CreatedById
                                              ,@CreatedBy
                                              ,@CreatedDate
                                              ,@ModifiedById
                                              ,@ModifiedBy
                                              ,@ModifiedDate
                                              ,@Status
                                            );

                                            SELECT EmployeeCode FROM Employee Where Id = (SELECT CAST(SCOPE_IDENTITY() as int))";
                        employee.Status = 1;
                        using var _connection = _context.CreateConnection();
                        _connection.Open();
                        employee.EmployeeCode = await _connection.ExecuteScalarAsync<int>(InsertQuery, employee);


                        var UserInsertUserQuery = @"INSERT INTO Users 
                                            (
                                              EmployeeCode,
                                              UserName,
                                              Email,
                                              PasswordHash,
                                              Salt,
                                              FirstName,
                                              LastName,
                                              Gender,
                                              UserRoles,
                                              IsActive,
                                              CreatedById,
                                              CreatedBy,
                                              CreatedDate,
                                              ModifiedById,
                                              ModifiedBy,
                                              ModifiedDate
                                            ) 
                                              VALUES 
                                            (
                                              @EmployeeCode,
                                              @Email,
                                              @Email,
                                              @PasswordHash,
                                              @Salt,
                                              @FirstName,
                                              @LastName,
                                              @Gender,
                                              @UserRoles,
                                              @IsActive,
                                              @CreatedById,
                                              @CreatedBy,
                                              @CreatedDate,
                                              @ModifiedById,
                                              @ModifiedBy,
                                              @ModifiedDate
                                             );";


                        var password = GenerateStrongPassword();
                        employee.Salt = PasswordHelper.GenerateSalt();
                        employee.PasswordHash = PasswordHelper.HashPassword(password, employee.Salt);

                        if (string.IsNullOrEmpty(employee.UserRoles))
                        {
                            employee.UserRoles = "4";
                        }
                        //user.DOB = DateTime.ParseExact(user.DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                        await _connection.ExecuteAsync(UserInsertUserQuery, employee);

                        var NotificationQuery = @"INSERT INTO Notifications 
                                                    (
                                                       NotificationType
                                                      ,NotificationDescription
                                                      ,NotificationUrl
                                                      ,EmployeeCode
                                                      ,CreatedDate
                                                      ,IsActive
                                                    )
                                                    VALUES 
                                                    (
                                                       @NotificationType
                                                      ,@NotificationDescription
                                                      ,@NotificationUrl
                                                      ,@EmployeeCode
                                                      ,@CreatedDate
                                                      ,@IsActive
                                                    );";

                        var notif = new Notifications();

                        notif.EmployeeCode = employee.EmployeeCode;
                        notif.NotificationType = "Profile";
                        notif.NotificationDescription = "Complete your Profile by visiting my profile.";
                        notif.NotificationUrl = "Employee/MyProfile";
                        notif.CreatedDate = DateTime.UtcNow;
                        notif.IsActive = true;
                        await _connection.ExecuteAsync(NotificationQuery, notif);


                        _connection.Close();

                        var from = _configuration["EmailConfigurations:SMTPUserName"];
                        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "NewEmployeePassword.html");
                        string htmlBody = System.IO.File.ReadAllText(templatePath)
                                              .Replace("{{UserEmail}}", employee.Email)
                                              .Replace("{{UserPassword}}", password);

                        await _email.SendEmailAsync(from, employee.FirstName + " " + employee.LastName, employee.Email, null, null, "Login Credentials", htmlBody, null, true, ',');

                    }
                }


                return Ok(new { StatusCode = HttpStatusCode.OK, ExcelData = employees, ExistingEmployees = existingEmployees });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }

        private List<Employee> ReadExcel(string filePath)
        {
            List<Employee> employees = new List<Employee>();

            var Departments = _common.GetAllAsync<Department>("Departments", HttpContext).GetAwaiter().GetResult();
            var Sites = _common.GetAllAsync<Sites>("Sites", HttpContext).GetAwaiter().GetResult();
            var Managers = _common.GetAllAsync<Employee>("Managers", HttpContext).GetAwaiter().GetResult();
            var EmployeePosition = _common.GetAllAsync<EmployeePosition>("EmployeePosition", HttpContext).GetAwaiter().GetResult();
            var EmployeeStatus = _common.GetAllAsync<EmployeeStatus>("EmployeeStatus", HttpContext).GetAwaiter().GetResult();
            var UserRoles = _common.GetAllAsync<Roles>("Roles", HttpContext).GetAwaiter().GetResult();

            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(filePath, false))
            {
                WorkbookPart workbookPart = doc.WorkbookPart;
                Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault();
                WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                bool firstRow = true;
                foreach (Row row in sheetData.Elements<Row>())
                {
                    if (firstRow) { firstRow = false; continue; } // Skip header row
                    var employee = new Employee();
                    var cells = row.Elements<Cell>().ToList();

                    employee.FirstName = GetCellValue(workbookPart, cells[0]);
                    employee.LastName = GetCellValue(workbookPart, cells[1]);
                    employee.Email = GetCellValue(workbookPart, cells[2]);
                    var DOB = GetCellValue(workbookPart, cells[3]);
                    employee.DOB = DateTime.ParseExact(DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    employee.PhoneNumber = GetCellValue(workbookPart, cells[4]);
                    var OnBoardingDate = GetCellValue(workbookPart, cells[5]);
                    employee.OnBoardingDate = DateTime.ParseExact(OnBoardingDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    employee.PositionName = GetCellValue(workbookPart, cells[6]);
                    var hiringDate = GetCellValue(workbookPart, cells[7]);
                    employee.HiringDate = DateTime.ParseExact(hiringDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    var EmployeeSiteNames = GetCellValue(workbookPart, cells[8]);
                    var EmployeeDepartmentNames = GetCellValue(workbookPart, cells[9]);

                    employee.EmploymentStatus = GetCellValue(workbookPart, cells[10]);
                    employee.ManagerName = GetCellValue(workbookPart, cells[11]);
                    employee.RoleName = GetCellValue(workbookPart, cells[12]);
                    employee.SinNo = GetCellValue(workbookPart, cells[13]);
                    var AcceptanceDate = GetCellValue(workbookPart, cells[14]);
                    employee.AcceptanceDate = DateTime.ParseExact(AcceptanceDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    employee.ImmigrationStatus = GetCellValue(workbookPart, cells[15]);
                    employee.WorkEligibility = GetCellValue(workbookPart, cells[16]);
                    employee.SponsorShip = GetCellValue(workbookPart, cells[17]);
                    employee.Other = GetCellValue(workbookPart, cells[18]);

                    var PositionId = EmployeePosition.Where(x => x.PositionName.ToLower() == employee.PositionName.ToLower()).Select(x => x.Id).FirstOrDefault();
                    if (!string.IsNullOrEmpty(EmployeeSiteNames) && !string.IsNullOrWhiteSpace(EmployeeSiteNames))
                    {
                        List<string> siteNames = EmployeeSiteNames.Split(',').Select(s => s.ToLower().Trim()).ToList();

                        var _sites = new List<Site>();

                        var sites = Sites.Where(x => siteNames.Contains(x.SiteName.ToLower())).ToList();

                        foreach (var site in sites)
                        {
                            var _site = new Site();
                            _site.id = Convert.ToInt32(site.Id);
                            _site.name = site.SiteName;
                            _sites.Add(_site);
                        }

                        if (_sites != null && _sites.Count > 0)
                        {
                            employee.SiteName = JsonConvert.SerializeObject(_sites);
                        }


                    }
                    else
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(EmployeeDepartmentNames) && !string.IsNullOrWhiteSpace(EmployeeDepartmentNames))
                    {
                        List<string> deptNames = EmployeeDepartmentNames.Split(',').Select(s => s.ToLower().Trim()).ToList();

                        var _depts = new List<Site>();

                        var depts = Departments.Where(x => deptNames.Contains(x.DepartmentName.ToLower())).ToList();

                        foreach (var dept in depts)
                        {
                            var _dept = new Site();
                            _dept.id = Convert.ToInt32(dept.DepartmentId);
                            _dept.name = dept.DepartmentName;
                            _depts.Add(_dept);
                        }

                        if (_depts != null && _depts.Count > 0)
                        {
                            employee.DepartmentName = JsonConvert.SerializeObject(_depts);
                        }

                    }
                    else
                    {
                        continue;
                    }
                    //var SiteId = Sites.Where(x => x.SiteName.ToLower() == employee.SiteName.ToLower()).Select(x => x.Id).FirstOrDefault();
                    var DepartmentId = Departments.Where(x => x.DepartmentName.ToLower() == employee.DepartmentName.ToLower()).Select(x => x.DepartmentId).FirstOrDefault();
                    var EmploymentStatusId = EmployeeStatus.Where(x => x.EmployeeStatusName.ToLower() == employee.EmploymentStatus.ToLower()).Select(x => x.Id).FirstOrDefault();
                    var ManagerId = Managers.Where(x => x.FirstName.ToLower() + " " + x.LastName.ToLower() == employee.ManagerName.ToLower()).Select(x => x.Id).FirstOrDefault();
                    var RoleId = UserRoles.Where(x => x.RoleName.ToLower() == employee.RoleName.ToLower()).Select(x => x.RoleId).FirstOrDefault();

                    if (PositionId != null)
                    {
                        employee.PositionId = PositionId;
                    }
                    else
                    {
                        continue;
                    }

                    //if (SiteId != null)
                    //{
                    //    employee.SiteId = SiteId;
                    //}
                    //else
                    //{
                    //    continue;
                    //}

                    if (DepartmentId != null)
                    {
                        employee.DepartmentId = DepartmentId;
                    }
                    else
                    {
                        continue;
                    }

                    if (EmploymentStatusId != null)
                    {
                        employee.EmploymentStatusId = EmploymentStatusId;
                    }
                    else
                    {
                        continue;
                    }


                    if (RoleId != null)
                    {
                        employee.RoleId = RoleId;
                        employee.UserRoles = RoleId.ToString();
                    }
                    else
                    {
                        continue;
                    }
                    var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                    var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                    employee.CreatedById = loggedinUserId;
                    employee.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                    employee.CreatedDate = DateTime.UtcNow;
                    employee.ModifiedById = loggedinUserId;
                    employee.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                    employee.ModifiedDate = DateTime.UtcNow;
                    employee.Status = 2;

                    employees.Add(employee);
                }
            }
            return employees;
        }

        private string GetCellValue(WorkbookPart workbookPart, Cell cell)
        {
            if (cell.CellValue == null) return "";
            string value = cell.CellValue.Text;

            if (cell.DataType?.Value == CellValues.SharedString)
            {
                value = workbookPart.SharedStringTablePart.SharedStringTable
                    .Elements<SharedStringItem>().ElementAt(
                        Convert.ToInt32(cell.CellValue.Text)).InnerText;
            }

            var cellText = (value ?? string.Empty).Trim();

            var cellWithType = new ExcelCellWithType();

            if (cell.StyleIndex != null)
            {
                var cellFormat = workbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ChildElements[
                    int.Parse(cell.StyleIndex.InnerText)] as CellFormat;

                if (cellFormat != null)
                {
                    cellWithType.ExcelCellFormat = cellFormat.NumberFormatId;

                    var dateFormat = GetDateTimeFormat(cellFormat.NumberFormatId);
                    if (!string.IsNullOrEmpty(dateFormat))
                    {
                        cellWithType.IsDateTimeType = true;

                        if (!string.IsNullOrEmpty(cellText))
                        {
                            if (double.TryParse(cellText, out var cellDouble))
                            {
                                var theDate = DateTime.FromOADate(cellDouble);
                                cellText = theDate.ToString(dateFormat);
                                value = cellText;
                            }
                        }
                    }
                }
            }

            cellWithType.Value = cellText;
            return value;
        }

        // https://msdn.microsoft.com/en-GB/library/documentformat.openxml.spreadsheet.numberingformat(v=office.14).aspx
        private readonly Dictionary<uint, string> DateFormatDictionary = new Dictionary<uint, string>()
        {
            [14] = "dd/MM/yyyy",
            [15] = "d-MMM-yy",
            [16] = "d-MMM",
            [17] = "MMM-yy",
            [18] = "h:mm AM/PM",
            [19] = "h:mm:ss AM/PM",
            [20] = "h:mm",
            [21] = "h:mm:ss",
            [22] = "M/d/yy h:mm",
            [30] = "M/d/yy",
            [34] = "yyyy-MM-dd",
            [45] = "mm:ss",
            [46] = "[h]:mm:ss",
            [47] = "mmss.0",
            [51] = "MM-dd",
            [52] = "yyyy-MM-dd",
            [53] = "yyyy-MM-dd",
            [55] = "yyyy-MM-dd",
            [56] = "yyyy-MM-dd",
            [58] = "MM-dd",
            [165] = "M/d/yy",
            [166] = "dd MMMM yyyy",
            [167] = "dd/MM/yyyy",
            [168] = "dd/MM/yy",
            [169] = "d.M.yy",
            [170] = "yyyy-MM-dd",
            [171] = "dd MMMM yyyy",
            [172] = "d MMMM yyyy",
            [173] = "M/d",
            [174] = "M/d/yy",
            [175] = "MM/dd/yy",
            [176] = "d-MMM",
            [177] = "d-MMM-yy",
            [178] = "dd-MMM-yy",
            [179] = "MMM-yy",
            [180] = "MMMM-yy",
            [181] = "MMMM d, yyyy",
            [182] = "M/d/yy hh:mm t",
            [183] = "M/d/y HH:mm",
            [184] = "MMM",
            [185] = "MMM-dd",
            [186] = "M/d/yyyy",
            [187] = "d-MMM-yyyy"
        };

        private string GetDateTimeFormat(UInt32Value numberFormatId)
        {
            return DateFormatDictionary.ContainsKey(numberFormatId) ? DateFormatDictionary[numberFormatId] : string.Empty;
        }


        private async Task SendProfileUpdateEmail(string employeeCode)
        {
            try
            {
                // Get employee email
                var emailQuery = @"SELECT Email FROM Employee WHERE EmployeeCode = @EmployeeCode";
                using var connection = _context.CreateConnection();
                connection.Open();
                var employeeDetails = await connection.QueryFirstOrDefaultAsync<Employee>(emailQuery, new { EmployeeCode = employeeCode });
                connection.Close();

                if (employeeDetails != null)
                {
                    var subject = $"Profile Details Updated - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <h2>Profile Details Updated</h2>
                            <p>Dear {employeeDetails.FirstName + " " + employeeDetails.LastName},</p>
                            <p>Your profile details has been updated.</p></br>
                            <p>Please log in to the HR Management System to view your profile details. <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a></p>
                            <p>Best regards,<br>Revital Health HR Management System</p><p><img src='cid:signatureImage' alt='Revital Health' style='height:60px; margin-top:10px;' /></p>
                        </body>
                        </html>";

                    await _email.SendEmailAsync(
                        from: "info@hrms.chestermerephysio.ca",
                        toname: employeeDetails.FirstName + " " + employeeDetails.LastName,
                        to: employeeDetails.Email,
                        cc: null,
                        bcc: null,
                        subject: subject,
                        body: body,
                        AttachmentsPaths: null,
                        IsBodyHtml: true,
                        delimitter: ';'
                    );
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
            }
        }
    }

    public class ImageUploadModel
    {
        [JsonProperty("base64")]
        public string Base64 { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }
    }
    public class ExcelCellWithType
    {
        public string Value { get; set; }
        public UInt32Value ExcelCellFormat { get; set; }
        public bool IsDateTimeType { get; set; }
    }
    public class Site
    {
        public int id { get; set; }
        public string name { get; set; }
    }

}
