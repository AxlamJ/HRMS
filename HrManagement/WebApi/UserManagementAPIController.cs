using Dapper;
using HrManagement.Data;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Globalization;

namespace HrManagement.WebApi
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementAPIController : ControllerBase
    {
        private readonly DataContext _context;
        public UserManagementAPIController(DataContext context)
        {
            _context = context;
        }


        [HttpPost("UpsertUsers")]
        public async Task<IActionResult> UpsertUsers(User user)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");


                if (user != null)
                {
                    using var connection = _context.CreateConnection();
                    var employeequery = "Select * from Employee Where EmployeeCode = @EmployeeCode";
                    var paramas = new { EmployeeCode = user.EmployeeCode };
                    connection.Open();
                    var employeedata = await connection.QueryFirstOrDefaultAsync<Employee>(employeequery, paramas);
                    connection.Close();

                    if (employeedata == null)
                    {
                        return StatusCode(409, new
                        {
                            StatusCode = 409,
                            Message = "Enter valid Employee Code"
                            //Message = "Site created successfully!",
                            //Data = new { Id = productId }
                        });

                    }
                    else
                    {

                        if (user.UserId == null)
                        {
                            using var _connection = _context.CreateConnection();
                            var _employeequery = "Select * from Employee Where EmployeeCode = @EmployeeCode";
                            var _paramas = new { EmployeeCode = user.EmployeeCode };
                            connection.Open();
                            var _employeedata = await connection.QueryFirstOrDefaultAsync<Employee>(employeequery, paramas);
                            connection.Close();

                            if (employeedata != null)
                            {
                                return StatusCode(409, new
                                {
                                    StatusCode = 409,
                                    Message = "This Employee Code is already entered in the system, use different Employee Code"
                                    //Message = "Site created successfully!",
                                    //Data = new { Id = productId }
                                });

                            }
                            else
                            {
                                var InsertUserQuery = @"INSERT INTO Users 
                                            (
                                              EmployeeCode,
                                              UserName,
                                              Email,
                                              PhoneNumber,
                                              PasswordHash,
                                              Salt,
                                              FirstName,
                                              LastName,
                                              Gender,
                                              UserSites,
                                              UserRoles,
                                              IsActive,
                                              CreatedById,
                                              CreatedBy,
                                              CreatedDate,
                                              ModifiedById,
                                              ModifiedBy,
                                              ModifiedDate,
                                              LastLoginDate
                                            ) 
                                              VALUES 
                                            (
                                              @EmployeeCode,
                                              @UserName,
                                              @Email,
                                              @PhoneNumber,
                                              @PasswordHash,
                                              @Salt,
                                              @FirstName,
                                              @LastName,
                                              @Gender,
                                              @UserSites,
                                              @UserRoles,
                                              @IsActive,
                                              @CreatedById,
                                              @CreatedBy,
                                              @CreatedDate,
                                              @ModifiedById,
                                              @ModifiedBy,
                                              @ModifiedDate,
                                              @LastLoginDate
                                             );";


                                user.Salt = PasswordHelper.GenerateSalt();
                                user.PasswordHash = PasswordHelper.HashPassword(user.Password, user.Salt);

                                //user.DOB = DateTime.ParseExact(user.DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                                user.CreatedById = loggedinUserId;
                                user.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                                user.CreatedDate = DateTime.UtcNow;
                                user.ModifiedById = loggedinUserId;
                                user.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                                user.ModifiedDate = DateTime.UtcNow;
                                user.IsActive = true;
                                connection.Open();
                                await connection.ExecuteAsync(InsertUserQuery, user);
                                connection.Close();

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
                            var UpdateQuery = @"UPDATE Users
                                            SET 
                                                UserName = @UserName,
                                                Email = @Email,
                                                PhoneNumber = @PhoneNumber,
                                                FirstName = @FirstName,
                                                LastName = @LastName,
                                                Gender = @Gender,
                                                UserSites = @UserSites,
                                                UserRoles = @UserRoles,
                                                IsActive = @IsActive,
                                                ModifiedById = @ModifiedById,
                                                ModifiedBy = @ModifiedBy,
                                                ModifiedDate = @ModifiedDate
                                                WHERE UserId = @UserId;";


                            user.Salt = PasswordHelper.GenerateSalt();
                            user.PasswordHash = PasswordHelper.HashPassword(user.Password, user.Salt);
                            //user.PasswordHash = PasswordHelper._HashPassword(user.Password);

                            //employee.DOB = DateTime.ParseExact(employee.DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            user.ModifiedById = loggedinUserId;
                            user.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                            user.ModifiedDate = DateTime.UtcNow;
                            user.IsActive = true;
                            connection.Open();
                            await connection.ExecuteAsync(UpdateQuery, user);
                            connection.Close();

                            return StatusCode(200, new
                            {
                                StatusCode = 200,
                                //Message = "Site created successfully!",
                                //Data = new { Id = productId }
                            });
                        }

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

        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(User user)
        {
            try
            {

                if (user != null)
                {
                    var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                    var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                    var UpdateQuery = @"UPDATE Users
                                            SET 
                                                PasswordHash = @PasswordHash,
                                                Salt = @Salt,
                                                ModifiedById = @ModifiedById,
                                                ModifiedBy = @ModifiedBy,
                                                ModifiedDate = @ModifiedDate
                                            WHERE UserId = @UserId;";


                    user.Salt = PasswordHelper.GenerateSalt();
                    user.PasswordHash = PasswordHelper.HashPassword(user.Password, user.Salt);

                    //employee.DOB = DateTime.ParseExact(employee.DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    user.ModifiedById = loggedinUserId;
                    user.ModifiedBy = String.Concat(loggedinUserFirstName, " ", loggedinUserLastName);
                    user.ModifiedDate = DateTime.UtcNow;
                    user.IsActive = true;
                    using var connection = _context.CreateConnection();
                    connection.Open();
                    await connection.ExecuteAsync(UpdateQuery, user);
                    connection.Close();



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

        [HttpGet("GetAllRoles")]
        public async Task<ActionResult> GetAllRoles()
        {
            try
            {
                var rolesquery = "Select * from Roles Where IsActive = 1";
                using var connection = _context.CreateConnection();
                connection.Open();
                var rolesData = await connection.QueryAsync<Roles>(rolesquery, null);
                var rolessList = rolesData.ToList<Roles>();

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    Roles = rolessList
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
        }

        [HttpPost("GetUsers")]
        public async Task<IActionResult> GetUsers(UsersFilter filters)
        {
            try
            {
                DataTableResponce dtr = new DataTableResponce();
                var tuple = await FilterUsersData(filters);
                dtr.iTotalRecords = tuple.Item2;
                dtr.iTotalDisplayRecords = tuple.Item2;
                dtr.aaData = tuple.Item1;
                return Ok(dtr);

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                throw ex;
            }
        }


        private async Task<Tuple<List<User>, int>> FilterUsersData(UsersFilter search)
        {
            //var timezoneoffset = Convert.ToInt32(HttpContext.Current.Request.Cookies["timeZoneOffsetCookie"].Value);

            try
            {


                var sqlParams = new List<SqlParameter>();
                var sqlParams1 = new List<SqlParameter>();

                var whereClause = $" Where U.IsActive = @IsActive";

                search.IsActive = true;


                if (!string.IsNullOrEmpty(search.FirstName))
                {
                    whereClause += "\n AND U.FirstName like '%'+@FirstName+'%'";
                    sqlParams.Add(new SqlParameter("@FirstName", search.FirstName));
                    sqlParams1.Add(new SqlParameter("@FirstName", search.FirstName));
                }
                if (!string.IsNullOrEmpty(search.LastName))
                {
                    whereClause += "\n AND U.LastName like '%'+@LastName+'%'";
                    sqlParams.Add(new SqlParameter("@LastName", search.LastName));
                    sqlParams1.Add(new SqlParameter("@LastName", search.LastName));
                }
                //if (search.DepartmentId != null)
                //{
                //    whereClause += "\n AND U.DepartmentId = @DepartmentId";
                //    sqlParams.Add(new SqlParameter("@DepartmentId", search.DepartmentId));
                //    sqlParams1.Add(new SqlParameter("@DepartmentId", search.DepartmentId));

                //}
                //if (search.SiteId != null)
                //{
                //    whereClause += "\n AND U.SiteId = @SiteId";
                //    sqlParams.Add(new SqlParameter("@SiteId", search.SiteId));
                //    sqlParams1.Add(new SqlParameter("@SiteId", search.SiteId));

                //}


                var orderByClause = " ";

                if (string.IsNullOrEmpty(search.SortCol))
                {
                    orderByClause = "\n ORDER BY U.ModifiedDate DESC";

                }
                else
                {
                    if (search.sSortDir_0.Equals("asc"))
                    {
                        if (search.SortCol.Equals("SiteName"))
                        {
                            orderByClause = "\n ORDER BY U.SiteName asc";
                        }
                        else if (search.SortCol.Equals("TimeZone"))
                        {
                            orderByClause = "\n ORDER BY U.TimeZone asc";

                        }
                        else if (search.SortCol.Equals("CountryName"))
                        {
                            orderByClause = "\n ORDER BY U.CountryName asc";
                        }
                    }
                    if (search.sSortDir_0.Equals("desc"))
                    {

                        if (search.SortCol.Equals("SiteName"))
                        {
                            orderByClause = "\n ORDER BY U.SiteName desc";
                        }
                        else if (search.SortCol.Equals("TimeZone"))
                        {
                            orderByClause = "\n ORDER BY U.TimeZone desc";

                        }
                        else if (search.SortCol.Equals("CountryName"))
                        {
                            orderByClause = "\n ORDER BY U.CountryName desc";
                        }
                    }

                }

                var joins = @"";

                var sqlForCount = @"SELECT COUNT(U.UserId) FROM dbo.Users U "
                   + joins
                   + whereClause;


                using var connection = _context.CreateConnection();
                connection.Open();
                var totalCount = await connection.QuerySingleAsync<int>(sqlForCount, search);
                //connection.Close();

                var sql = @"SELECT 
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
                           ,U.UserSites
                           ,U.UserRoles
                           ,U.IsActive
                           ,U.CreatedById
                           ,U.CreatedBy
                           ,U.CreatedDate
                           ,U.ModifiedById
                           ,U.ModifiedBy
                           ,U.ModifiedDate
                           ,U.LastLoginDate
                            FROM Users U"
                            + joins
                            + whereClause
                            + orderByClause
                            + "\n OFFSET @iDisplayStart ROWS FETCH NEXT @iDisplayLength ROWS ONLY;";
                var data = await connection.QueryAsync<User>(sql, search);
                var EmployeeList = data.ToList<User>();
                //var data = db.Database.SqlQuery<Sites>(sql, sqlParams.ToArray()).ToList();
                return new Tuple<List<User>, int>(EmployeeList, totalCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost("DeleteUserById")]
        public async Task<ActionResult> DeleteUserById(string Id)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");
                var userequery = @"Update Users SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                   Where UserId = @Id";
                var paramas = new { Id = Id, ModifiedById = loggedinUserId, ModifiedBy = string.Concat(loggedinUserFirstName, " ", loggedinUserLastName), ModifiedDate = DateTime.UtcNow, IsActive = false };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(userequery, paramas);
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
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
        }


        [HttpGet("GetUserById")]
        public async Task<ActionResult> GetUserById(string UserId)
        {
            try
            {
                var userquery = "Select * from Users Where UserId = @UserId";
                var paramas = new { UserId = UserId };
                using var connection = _context.CreateConnection();
                connection.Open();
                var userdata = await connection.QueryFirstOrDefaultAsync<User>(userquery, paramas);
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    User = userdata
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
        }
    }
}
