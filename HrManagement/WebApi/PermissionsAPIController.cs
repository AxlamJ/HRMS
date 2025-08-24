using Dapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using HrManagement.Data;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.IO;

namespace HrManagement.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsAPIController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly Email _email;

        public PermissionsAPIController(DataContext context, Email email)
        {
            _context = context;
            _email = email;
        }


        [HttpPost("UpsertRoleDetails")]
        public async Task<IActionResult> UpsertRoleDetails(PermissionRoles roleInfo)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");


                if (roleInfo != null)
                {
                    if (roleInfo.RoleId == null)
                    {
                        var InsertQuery = @"INSERT INTO Roles (
                                            RoleName
                                           ,Description
                                           ,RoleResources
                                           ,CreatedById
                                           ,CreatedBy
                                           ,CreatedDate
                                           ,ModifiedById
                                           ,ModifiedBy
                                           ,ModifiedDate
                                           ,IsActive
                                           ,IsActiveApproved) 
                                           VALUES (
                                            @RoleName
                                           ,@Description
                                           ,@RoleResources
                                           ,@CreatedById
                                           ,@CreatedBy
                                           ,@CreatedDate
                                           ,@ModifiedById
                                           ,@ModifiedBy
                                           ,@ModifiedDate
                                           ,@IsActive
                                           ,@IsActiveApproved)";

                        roleInfo.CreatedById = loggedinUserId;
                        roleInfo.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        roleInfo.CreatedDate = DateTime.UtcNow;
                        roleInfo.ModifiedById = loggedinUserId;
                        roleInfo.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        roleInfo.ModifiedDate = DateTime.UtcNow;
                        roleInfo.IsActive = true;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(InsertQuery, roleInfo);
                        connection.Close();
                    }
                    else
                    {
                        var UpdateQuery = @"UPDATE Roles SET
                                                RoleName = @RoleName
                                               ,Description = @Description
                                               ,RoleResources = @RoleResources
                                               ,ModifiedById = @ModifiedById 
                                               ,ModifiedBy = @ModifiedBy 
                                               ,ModifiedDate = @ModifiedDate 
                                               ,IsActive = @IsActive 
                                                WHERE RoleId = @RoleId";
                        roleInfo.ModifiedById = loggedinUserId;
                        roleInfo.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        roleInfo.ModifiedDate = DateTime.UtcNow;
                        roleInfo.IsActive = true;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(UpdateQuery, roleInfo);
                        connection.Close();
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

        [HttpPost("UpsertUsersRole")]
        public async Task<IActionResult> UpsertUsersRole(List<UserRoles> usersRoles)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");


                if (usersRoles != null && usersRoles.Count > 0)
                {
                    foreach (var user in usersRoles)
                    {
                        user.ModifiedById = loggedinUserId;
                        user.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        user.ModifiedDate = DateTime.UtcNow;
                    }

                    var UpdateQuery = @"UPDATE Users SET
                                                UserRoles = @RoleId
                                               ,ModifiedById = @ModifiedById 
                                               ,ModifiedBy = @ModifiedBy 
                                               ,ModifiedDate = @ModifiedDate 
                                                WHERE UserId = @UserId";
                    using var connection = _context.CreateConnection();
                    connection.Open();
                    await connection.ExecuteAsync(UpdateQuery, usersRoles);
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

        [HttpGet("GetRoleById")]
        public async Task<ActionResult> GetRoleById(string RoleId)
        {
            try
            {
                var departmentquery = "Select * from Roles Where RoleId = @RoleId";
                var paramas = new { RoleId = RoleId };
                using var connection = _context.CreateConnection();
                connection.Open();
                var roledata = await connection.QueryFirstOrDefaultAsync<PermissionRoles>(departmentquery, paramas);
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    RoleData = roledata
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

        [HttpPost("DeleteRoleById")]
        public async Task<ActionResult> DeleteRoleById(string RoleId)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var departmentquery = @"Update Roles SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                   Where RoleId = @RoleId";
                var paramas = new { RoleId = RoleId, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(departmentquery, paramas);
                connection.Close();



                var query = "Select * from Roles where RoleId = @RoleId";
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var rolesdata = await connection.QueryFirstOrDefaultAsync<PermissionRoles>(query, paramas);
                _connection.Close();

                await SendDeletionEmail(rolesdata);

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

        [HttpPost("ApproveDeleteRoleById")]
        public async Task<ActionResult> ApproveDeleteRoleById(string RoleId)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");


                var query = @"Select E.Email,E.FirstName, E.LastName from Roles R 
                            JOIN USERS U ON U.UserId = R.ModifiedById
                            JOIN Employee E ON E.EmployeeCode = U.EmployeeCode
                            Where R.RoleId = @RoleId";

                var _params = new { RoleId = RoleId };
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var employee = await _connection.QueryFirstOrDefaultAsync<Employee>(query, _params);
                _connection.Close();


                var rolequery = @"Update Roles SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                  ,IsActiveApproved = @IsActiveApproved 
                                   Where RoleId = @RoleId";
                var paramas = new { RoleId = RoleId, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false,IsActiveApproved = true };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(rolequery, paramas);


                var roledataquery = "Select * from Roles where RoleId = @RoleId";
                var rolesdata = await connection.QueryFirstOrDefaultAsync<PermissionRoles>(roledataquery, paramas);
                connection.Close();

                await SendDeletionApprovalEmail(employee, rolesdata);

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

        [HttpPost("GetRoles")]
        public async Task<IActionResult> GetRoles(PermissionRolesFilters filters)
        {
            try
            {
                DataTableResponce dtr = new DataTableResponce();
                var tuple = await FilterRolesData(filters);
                dtr.iTotalRecords = tuple.Item2;
                dtr.iTotalDisplayRecords = tuple.Item2;
                dtr.aaData = tuple.Item1;
                return Ok(dtr);

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
        private async Task<Tuple<List<PermissionRoles>, int>> FilterRolesData(PermissionRolesFilters search)
        {
            //var timezoneoffset = Convert.ToInt32(HttpContext.Current.Request.Cookies["timeZoneOffsetCookie"].Value);

            try
            {
                var sqlParams = new List<SqlParameter>();
                var sqlParams1 = new List<SqlParameter>();

                var whereClause = $" Where R.IsActiveApproved = 0";

                search.IsActive = true;


                if (!string.IsNullOrEmpty(search.RoleName))
                {
                    whereClause += "\n AND R.RoleName like '%'+@RoleName+'%'";
                    sqlParams.Add(new SqlParameter("@RoleName", search.RoleName));
                    sqlParams1.Add(new SqlParameter("@RoleName", search.RoleName));
                }


                var orderByClause = "\n ORDER BY R.RoleName Asc";


                var joins = @"";

                var sqlForCount = @"SELECT COUNT(R.RoleId) FROM Roles R "
                   + joins
                   + whereClause;


                using var connection = _context.CreateConnection();
                connection.Open();
                var totalCount = await connection.QuerySingleAsync<int>(sqlForCount, search);
                //connection.Close();

                var sql = @"SELECT 
	                         R.RoleId
                            ,R.RoleName
                            ,R.Description
                            ,R.RoleResources
                            ,R.CreatedById
                            ,R.CreatedBy
                            ,R.CreatedDate
                            ,R.ModifiedById
                            ,R.ModifiedBy
                            ,R.ModifiedDate
                            ,R.IsActive
                            FROM Roles R"
                            + joins
                            + whereClause
                            + orderByClause
                            + "\n OFFSET @iDisplayStart ROWS FETCH NEXT @iDisplayLength ROWS ONLY;";
                var data = await connection.QueryAsync<PermissionRoles>(sql, search);
                var RolesList = data.ToList<PermissionRoles>();
                //var data = db.Database.SqlQuery<Sites>(sql, sqlParams.ToArray()).ToList();
                return new Tuple<List<PermissionRoles>, int>(RolesList, totalCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }



        [HttpPost("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(UsersFilter filters)
        {
            try
            {
                DataTableResponce dtr = new DataTableResponce();
                var tuple = await FilterAllUsersData(filters);
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


        private async Task<Tuple<List<User>, int>> FilterAllUsersData(UsersFilter search)
        {

            try
            {


                var sqlParams = new List<SqlParameter>();
                var sqlParams1 = new List<SqlParameter>();

                var whereClause = $" Where U.IsActive = @IsActive";

                search.IsActive = true;

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


                var orderByClause = "\n ORDER BY Concat(U.FirstName,' ',U.LastName) ASC";

                var joins = @" JOIN Roles R ON R.RoleId = U.UserRoles";

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
                           ,R.RoleName
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


        private async Task SendDeletionEmail(PermissionRoles role)
        {
            try
            {
                // Get employee email
                var emailAdminQuery = @"SELECT E.FirstName,E.LastName,E.EMail FROM Employee E 
                                        JOIN USERS U ON U.EmployeeCode = E.EmployeeCode 
                                        JOIN Roles R ON R.RoleId = U.UserRoles  
                                        WHERE R.RoleName like '%Admin%' AND U.IsActive = 1 AND E.Status = 1";
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var adminDetails = await _connection.QueryAsync<Employee>(emailAdminQuery, null);
                var adminslist = adminDetails.ToList<Employee>();
                _connection.Close();
                Dictionary<string, string> toEmails = new Dictionary<string, string>();

                foreach (var admin in adminslist)
                {
                    toEmails.Add(admin.FirstName + " " + admin.LastName, admin.Email);
                }
                if (adminslist != null)
                {
                    var subject = $"Roles Deletion Request - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear All,</p></br>
                            <p>{role.ModifiedBy} has requested to delete the role named {role.RoleName}</p></br>
                            <p>Access the HRMS at : <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a> and Navigate to Permissions to approve the deletion request.</p></br></br>
                            <p>Please access HR System at: <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a></p>
                            <p>Best regards,<br>Revital Health HR Management System</p><p><img src='cid:signatureImage' alt='Revital Health' style='height:60px; margin-top:10px;'/></p>
                        </body>
                        </html>";

                    await _email.SendEmailMultipleAsync(
                        from: "info@hrms.chestermerephysio.ca",
                        to: toEmails,
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

        private async Task SendDeletionApprovalEmail(Employee employee, PermissionRoles role)
        {
            try
            {

                if (employee != null)
                {
                    var subject = $"Role Deletion Request Approved - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear {employee.FirstName + " " + employee.LastName},</p></br>
                            <p>{role.ModifiedBy} has approved the request to delete the role named {role.RoleName} and it is deleted successfully from the system.</p></br>
                            <p>Please access HR System at: <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a></p>
                            <p>Best regards,<br>Revital Health HR Management System</p><p><img src='cid:signatureImage' alt='Revital Health' style='height:60px; margin-top:10px;'/></p>
                        </body>
                        </html>";

                    await _email.SendEmailAsync(
                        from: "info@hrms.chestermerephysio.ca",
                        to: employee.Email,
                        toname: employee.FirstName + " " + employee.LastName,
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
}
