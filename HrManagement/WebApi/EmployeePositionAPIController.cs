using Dapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using HrManagement.Controllers;
using HrManagement.Data;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace HrManagement.WebApi
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeePositionAPIController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly Email _email;
        public EmployeePositionAPIController(DataContext context, Email email)
        {
            _context = context;
            _email = email;
        }

        [HttpPost("UpsertPosition")]
        public async Task<IActionResult> UpsertPosition(EmployeePosition position)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");


                if (position != null)
                {
                    if (position.Id == null)
                    {
                        var InsertQuery = @"INSERT INTO EmployeePosition (
                                             PositionName
                                            ,CreatedBy
                                            ,CreatedById
                                            ,CreatedDate
                                            ,ModifiedBy
                                            ,ModifiedById
                                            ,ModifiedDate
                                            ,IsActive
                                            ,IsActiveApproved
                                          ) 
                                           VALUES 
                                          (
                                            @PositionName
                                           ,@CreatedBy
                                           ,@CreatedById
                                           ,@CreatedDate
                                           ,@ModifiedBy
                                           ,@ModifiedById
                                           ,@ModifiedDate
                                           ,@IsActive
                                           ,@IsActiveApproved
                                          )";

                        position.CreatedById = loggedinUserId;
                        position.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        position.CreatedDate = DateTime.UtcNow;
                        position.ModifiedById = loggedinUserId;
                        position.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        position.ModifiedDate = DateTime.UtcNow;
                        position.IsActive = true;
                        position.IsActiveApproved = false;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(InsertQuery, position);
                        connection.Close();
                    }
                    else
                    {
                        var UpdateQuery = @"UPDATE EmployeePosition SET
                                                PositionName = @PositionName  
                                               ,ModifiedById = @ModifiedById 
                                               ,ModifiedBy = @ModifiedBy 
                                               ,ModifiedDate = @ModifiedDate 
                                               ,IsActive = @IsActive 
                                                WHERE Id = @Id";
                        position.ModifiedById = loggedinUserId;
                        position.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        position.ModifiedDate = DateTime.UtcNow;
                        position.IsActive = true;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(UpdateQuery, position);
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

        [HttpGet("GetPositionById")]
        public async Task<ActionResult> GetPositionById(string Id)
        {
            try
            {
                var query = "Select * from EmployeePosition Where Id = @Id";
                var paramas = new { Id = Id };
                using var connection = _context.CreateConnection();
                connection.Open();
                var positiondata = await connection.QueryFirstOrDefaultAsync<EmployeePosition>(query, paramas);
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    Position = positiondata
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

        [HttpPost("DeletePositionById")]
        public async Task<ActionResult> DeletePositionById(string Id)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var departmentquery = @"Update EmployeePosition SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                   Where Id = @Id";
                var paramas = new { Id = Id, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(departmentquery, paramas);
                connection.Close();


                var query = "Select * from EmployeePosition where Id = @Id";
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var positionData = await connection.QueryFirstOrDefaultAsync<EmployeePosition>(query, paramas);
                _connection.Close();

                await SendDeletionEmail(positionData);

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

        [HttpPost("ApproveDeletePositionById")]
        public async Task<ActionResult> ApproveDeletePositionById(string Id)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var _query = @"Select E.Email,E.FirstName, E.LastName from EmployeePosition EP 
                            JOIN USERS U ON U.UserId = EP.ModifiedById
                            JOIN Employee E ON E.EmployeeCode = U.EmployeeCode
                            Where EP.Id = @Id";

                var _params = new { Id = Id };
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var employee = await _connection.QueryFirstOrDefaultAsync<Employee>(_query, _params);
                _connection.Close();


                var query = @"Update EmployeePosition SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                  ,IsActiveApproved = @IsActiveApproved 
                                   Where Id = @Id";
                var paramas = new { Id = Id, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false , IsActiveApproved  = true};
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(query, paramas);
                connection.Close();



                var __query = "Select * from EmployeePosition where Id = @Id";
                using var __connection = _context.CreateConnection();
                __connection.Open();
                var positiondata = await connection.QueryFirstOrDefaultAsync<EmployeePosition>(_query, paramas);
                __connection.Close();

                await SendDeletionApprovalEmail(employee, positiondata);




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

        [HttpPost("GetPositions")]
        public async Task<IActionResult> GetPositions(EmployeePositionFilter filters)
        {
            try
            {
                DataTableResponce dtr = new DataTableResponce();
                var tuple = await FilterPositionsData(filters);
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
        private async Task<Tuple<List<EmployeePosition>, int>> FilterPositionsData(EmployeePositionFilter search)
        {
            //var timezoneoffset = Convert.ToInt32(HttpContext.Current.Request.Cookies["timeZoneOffsetCookie"].Value);

            try
            {
                var sqlParams = new List<SqlParameter>();
                var sqlParams1 = new List<SqlParameter>();

                var whereClause = $" Where P.IsActiveApproved = 0";

                search.IsActive = true;


                if (!string.IsNullOrEmpty(search.PositionName))
                {
                    whereClause += "\n AND P.PositionName like '%'+@PositionName+'%'";
                    sqlParams.Add(new SqlParameter("@PositionName", search.PositionName));
                    sqlParams1.Add(new SqlParameter("@PositionName", search.PositionName));
                }


                var orderByClause = " ";

                if (string.IsNullOrEmpty(search.SortCol))
                {
                    orderByClause = "\n ORDER BY P.ModifiedDate DESC";

                }
                else
                {
                    if (search.sSortDir_0.Equals("asc"))
                    {
                        if (search.SortCol.Equals("PositionName"))
                        {
                            orderByClause = "\n ORDER BY P.PositionName asc";
                        }
                    }
                    if (search.sSortDir_0.Equals("desc"))
                    {

                        if (search.SortCol.Equals("PositionName"))
                        {
                            orderByClause = "\n ORDER BY P.PositionName desc";
                        }
                    }

                }

                var joins = @"";

                var sqlForCount = @"SELECT COUNT(P.Id) FROM EmployeePosition P "
                   + joins
                   + whereClause;


                using var connection = _context.CreateConnection();
                connection.Open();
                var totalCount = await connection.QuerySingleAsync<int>(sqlForCount, search);
                //connection.Close();

                var sql = @"SELECT 
                             P.Id
                            ,P.PositionName
                            ,P.CreatedBy
                            ,P.CreatedById
                            ,P.CreatedDate
                            ,P.ModifiedBy
                            ,P.ModifiedById
                            ,P.ModifiedDate
                            ,P.IsActive
                            FROM dbo.EmployeePosition P"
                            + joins
                            + whereClause
                            + orderByClause
                            + "\n OFFSET @iDisplayStart ROWS FETCH NEXT @iDisplayLength ROWS ONLY;";
                var data = await connection.QueryAsync<EmployeePosition>(sql, search);
                var PositionsList = data.ToList<EmployeePosition>();
                //var data = db.Database.SqlQuery<Sites>(sql, sqlParams.ToArray()).ToList();
                return new Tuple<List<EmployeePosition>, int>(PositionsList, totalCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }



        private async Task SendDeletionEmail(EmployeePosition emppos)
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
                    var subject = $"Position Deletion Request - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear All,</p></br>
                            <p>{emppos.ModifiedBy} has requested to delete the Position named {emppos.PositionName}</p></br>
                            <p>Access the HRMS at : <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a> and Navigate to Manage Positions to approve the deletion request.</p></br></br>
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

        private async Task SendDeletionApprovalEmail(Employee employee, EmployeePosition emppos)
        {
            try
            {

                if (employee != null)
                {
                    var subject = $"Position Deletion Request Approved - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear {employee.FirstName + " " + employee.LastName},</p></br>
                            <p>{emppos.ModifiedBy} has approved the request to delete the position named {emppos.PositionName} and it is deleted successfully from the system.</p></br>
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
