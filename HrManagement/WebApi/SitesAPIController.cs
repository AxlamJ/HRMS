using Dapper;
using HrManagement.Data;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Net;
using System.Transactions;

namespace HrManagement.WebApi
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SitesAPIController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly Email _email;
        public SitesAPIController(DataContext context, Email email)
        {
            _context = context;
            _email = email;
        }

        [HttpPost("UpsertSite")]
        public async Task<IActionResult> UpsertSite(Sites site)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");


                if (site != null)
                {
                    if (site.Id == null)
                    {
                        var InsertQuery = @"INSERT INTO Sites (
                                            SiteName
                                           ,CountryId
                                           ,CountryName
                                           ,TimeZoneId
                                           ,TimeZoneName
                                           ,TimeZoneOffset
                                           ,CreatedById
                                           ,CreatedBy
                                           ,CreatedDate
                                           ,ModifiedById
                                           ,ModifiedBy
                                           ,ModifiedDate
                                           ,IsActive 
                                           ,IsActiveApproved) 
                                           VALUES (
                                            @SiteName
                                           ,@CountryId
                                           ,@CountryName
                                           ,@TimeZoneId
                                           ,@TimeZoneName
                                           ,@TimeZoneOffset
                                           ,@CreatedById
                                           ,@CreatedBy
                                           ,@CreatedDate
                                           ,@ModifiedById
                                           ,@ModifiedBy
                                           ,@ModifiedDate
                                           ,@IsActive
                                           ,@IsActiveApproved)";

                        site.CreatedById = loggedinUserId;
                        site.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        site.CreatedDate = DateTime.UtcNow;
                        site.ModifiedById = loggedinUserId;
                        site.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        site.ModifiedDate = DateTime.UtcNow;
                        site.IsActive = true;
                        site.IsActiveApproved = false;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(InsertQuery, site);
                        connection.Close();
                    }
                    else
                    {
                        var UpdateQuery = @"UPDATE Sites SET
                                                SiteName = @SiteName  
                                               ,CountryId = @CountryId 
                                               ,CountryName = @CountryName 
                                               ,TimeZoneId = @TimeZoneId 
                                               ,TimeZoneName = @TimeZoneName 
                                               ,TimeZoneOffset = @TimeZoneOffset 
                                               ,CreatedById = @CreatedById 
                                               ,CreatedBy = @CreatedBy 
                                               ,CreatedDate = @CreatedDate 
                                               ,ModifiedById = @ModifiedById 
                                               ,ModifiedBy = @ModifiedBy 
                                               ,ModifiedDate = @ModifiedDate 
                                               ,IsActive = @IsActive 
                                                WHERE Id = @Id";
                        site.ModifiedById = loggedinUserId;
                        site.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        site.ModifiedDate = DateTime.UtcNow;
                        site.IsActive = true;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(UpdateQuery, site);
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

        [HttpGet("GetSiteById")]
        public async Task<ActionResult> GetSiteById(string Id)
        {
            try
            {
                var sitequery = "Select * from Sites Where Id = @Id";
                var paramas = new { Id = Id };
                using var connection = _context.CreateConnection();
                connection.Open();
                var sitedata = await connection.QueryFirstOrDefaultAsync<Sites>(sitequery, paramas);
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    Site = sitedata
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


        [HttpPost("DeleteSiteById")]
        public async Task<ActionResult> DeleteSiteById(string Id)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var sitequery = @"Update Sites SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                   Where Id = @Id";
                var paramas = new { Id = Id, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(sitequery, paramas);
                connection.Close();

                var query = "Select * from Sites where Id = @Id";
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var sitedata = await connection.QueryFirstOrDefaultAsync<Sites>(query, paramas);
                _connection.Close();

                await SendDeletionEmail(sitedata);
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


        [HttpPost("ApproveDeleteSiteById")]
        public async Task<ActionResult> ApproveDeleteSiteById(string Id)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");


                var query = @"Select E.Email,E.FirstName, E.LastName from Sites S 
                            JOIN USERS U ON U.UserId = S.ModifiedById
                            JOIN Employee E ON E.EmployeeCode = U.EmployeeCode
                            Where S.Id = @Id";

                var _params = new { Id = Id };
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var employee = await _connection.QueryFirstOrDefaultAsync<Employee>(query, _params);
                _connection.Close();


                var sitequery = @"Update Sites SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                  ,IsActiveApproved = @IsActiveApproved
                                   Where Id = @Id";
                var __params = new { Id = Id, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false, IsActiveApproved = true };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(sitequery, __params);
                connection.Close();

                var _query = "Select * from Sites where Id = @Id";
                using var __connection = _context.CreateConnection();
                __connection.Open();
                var sitedata = await connection.QueryFirstOrDefaultAsync<Sites>(_query, __params);
                __connection.Close();

                await SendDeletionApprovalEmail(employee, sitedata);
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

        [HttpPost("GetSites")]
        public async Task<IActionResult> GetSites(SitesFilter filters)
        {
            try
            {
                DataTableResponce dtr = new DataTableResponce();
                var tuple = await FilterSitesData(filters);
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


        private async Task<Tuple<List<Sites>, int>> FilterSitesData(SitesFilter search)
        {
            //var timezoneoffset = Convert.ToInt32(HttpContext.Current.Request.Cookies["timeZoneOffsetCookie"].Value);
            try
            {


                var sqlParams = new List<SqlParameter>();
                var sqlParams1 = new List<SqlParameter>();

                var whereClause = $" Where S.IsActiveApproved = 0";

                search.IsActive = true;


                if (!string.IsNullOrEmpty(search.SiteName))
                {
                    whereClause += "\n AND S.SiteName like '%'+@SiteName+'%'";
                    sqlParams.Add(new SqlParameter("@SiteName", search.SiteName));
                    sqlParams1.Add(new SqlParameter("@SiteName", search.SiteName));
                }
                if (!string.IsNullOrEmpty(search.CountryId))
                {
                    whereClause += "\n AND S.CountryId = @CountryId";
                    sqlParams.Add(new SqlParameter("@CountryId", search.CountryId));
                    sqlParams1.Add(new SqlParameter("@CountryId", search.CountryId));

                }
                if (!string.IsNullOrEmpty(search.TimeZoneId))
                {
                    whereClause += "\n AND S.TimeZoneId = @TimeZoneId";
                    sqlParams.Add(new SqlParameter("@TimeZoneId", search.TimeZoneId));
                    sqlParams1.Add(new SqlParameter("@TimeZoneId", search.TimeZoneId));

                }

                var orderByClause = " ";

                if (string.IsNullOrEmpty(search.SortCol))
                {
                    orderByClause = "\n ORDER BY S.ModifiedDate DESC";

                }
                else
                {
                    if (search.sSortDir_0.Equals("asc"))
                    {
                        if (search.SortCol.Equals("SiteName"))
                        {
                            orderByClause = "\n ORDER BY S.SiteName asc";
                        }
                        else if (search.SortCol.Equals("TimeZone"))
                        {
                            orderByClause = "\n ORDER BY S.TimeZone asc";

                        }
                        else if (search.SortCol.Equals("CountryName"))
                        {
                            orderByClause = "\n ORDER BY S.CountryName asc";
                        }
                    }
                    if (search.sSortDir_0.Equals("desc"))
                    {

                        if (search.SortCol.Equals("SiteName"))
                        {
                            orderByClause = "\n ORDER BY S.SiteName desc";
                        }
                        else if (search.SortCol.Equals("TimeZone"))
                        {
                            orderByClause = "\n ORDER BY S.TimeZone desc";

                        }
                        else if (search.SortCol.Equals("CountryName"))
                        {
                            orderByClause = "\n ORDER BY S.CountryName desc";
                        }
                    }

                }

                var joins = @"";

                var sqlForCount = @"SELECT COUNT(S.Id) FROM dbo.Sites S "
                   + joins
                   + whereClause;


                using var connection = _context.CreateConnection();
                connection.Open();
                var totalCount = await connection.QuerySingleAsync<int>(sqlForCount, search);
                //connection.Close();

                var sql = @"SELECT 
                               S.Id
                              ,S.SiteName
                              ,S.CountryId
                              ,S.CountryName
                              ,S.TimeZoneId
                              ,S.TimeZoneName
                              ,S.TimeZoneOffset
                              ,S.CreatedById
                              ,S.CreatedBy
                              ,S.CreatedDate
                              ,S.ModifiedById
                              ,S.ModifiedBy
                              ,S.ModifiedDate
                              ,S.IsActive
                              ,S.IsActiveApproved
                            FROM dbo.Sites S"
                            + joins
                            + whereClause
                            + orderByClause
                            + "\n OFFSET @iDisplayStart ROWS FETCH NEXT @iDisplayLength ROWS ONLY;";
                var data = await connection.QueryAsync<Sites>(sql, search);
                var SitesList = data.ToList<Sites>();
                //var data = db.Database.SqlQuery<Sites>(sql, sqlParams.ToArray()).ToList();
                return new Tuple<List<Sites>, int>(SitesList, totalCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task SendDeletionEmail(Sites site)
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
                    var subject = $"Site Deletion Request - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear All,</p></br>
                            <p>{site.ModifiedBy} has requested to delete the site named {site.SiteName}</p></br>
                            <p>Access the HRMS at : <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a> and Navigate to Manage Sites to approve the deletion request.</p></br></br>
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
        private async Task SendDeletionApprovalEmail(Employee employee,Sites site)
        {
            try
            {

                if (employee != null)
                {
                    var subject = $"Site Deletion Request Approved - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear {employee.FirstName +" "+employee.LastName },</p></br>
                            <p>{site.ModifiedBy} has approved the request to delete the site named {site.SiteName} and it is deleted successfully from the system.</p></br>
                            <p>Please access HR System at: <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a></p>
                            <p>Best regards,<br>Revital Health HR Management System</p><p><img src='cid:signatureImage' alt='Revital Health' style='height:60px; margin-top:10px;'/></p>
                        </body>
                        </html>";

                    await _email.SendEmailAsync(
                        from: "info@hrms.chestermerephysio.ca",
                        to: employee.Email,
                        toname: employee.FirstName + " "+employee.LastName,
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
