using Dapper;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office2010.Excel;
using HrManagement.Data;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Globalization;

namespace HrManagement.WebApi
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LeavesAPIController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly Email _email;

        public LeavesAPIController(DataContext context, Email email)
        {
            _context = context;
            _email = email;
        }

        [HttpPost("UpsertLeave")]
        public async Task<IActionResult> UpsertLeave(Leaves leave)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");
                var EmployeeCode = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeCode"));


                if (leave != null)
                {
                    if (leave.LeaveId == null)
                    {
                        var InsertQuery = @"INSERT INTO Leaves
                                            (
                                                UserId,
                                                EmployeeCode,
                                                EmployeeName,
                                                LeaveTypeId,
                                                LeaveTypeName,
                                                LeaveTypeShortName,
                                                StartDate,
                                                EndDate,
                                                TotalDays,
                                                LeaveStatusId,
                                                LeaveStatusName,
                                                LeaveReason,
                                                ApprovedById,
                                                ApprovedBy,
                                                ApprovedDate,
                                                Comments,
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
                                                @UserId,
                                                @EmployeeCode,
                                                @EmployeeName,
                                                @LeaveTypeId,
                                                @LeaveTypeName,
                                                @LeaveTypeShortName,
                                                @StartDate,
                                                @EndDate,
                                                @TotalDays,
                                                @LeaveStatusId,
                                                @LeaveStatusName,
                                                @LeaveReason,
                                                @ApprovedById,
                                                @ApprovedBy,
                                                @ApprovedDate,
                                                @Comments,
                                                @CreatedById,
                                                @CreatedBy,
                                                @CreatedDate,
                                                @ModifiedById,
                                                @ModifiedBy,
                                                @ModifiedDate,
                                                @IsActive
                                            );
                                            
                                            SELECT CAST(SCOPE_IDENTITY() AS INT);";


                        leave.StartDate = DateTime.ParseExact(leave.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        leave.EndDate = DateTime.ParseExact(leave.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                        var _StartDate = DateTime.ParseExact(leave.StartDate + " 00:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        var _EndDate = DateTime.ParseExact(leave.EndDate + " 23:59:59", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                        var daysdiff = Convert.ToInt32((_EndDate - _StartDate).TotalDays);

                        var isAutoApproveQuery = @"Select * from LeaveTypes where LeaveTypeId = @LeaveTypeId and IsActive = 1";
                        using var _connection = _context.CreateConnection();
                        _connection.Open();
                        var leavetype = await _connection.QueryFirstOrDefaultAsync<LeaveTypes>(isAutoApproveQuery, leave);
                        _connection.Close();

                        if (leavetype != null && leavetype.AutoApprove == true)
                        {
                            leave.LeaveStatusId = 2;
                            leave.LeaveStatusName = "Approved";
                        }
                        else
                        {
                            leave.LeaveStatusId = 1;
                            leave.LeaveStatusName = "Pending";
                        }

                        leave.UserId = loggedinUserId;
                        leave.EmployeeCode = EmployeeCode;
                        leave.TotalDays = daysdiff;
                        leave.CreatedById = loggedinUserId;
                        leave.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        leave.CreatedDate = DateTime.UtcNow;
                        leave.ModifiedById = loggedinUserId;
                        leave.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        leave.ModifiedDate = DateTime.UtcNow;
                        leave.IsActive = true;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        var leavedId = await connection.ExecuteScalarAsync<int>(InsertQuery, leave);
                        connection.Close();

                        await SendLeaveAddedEmail(leave.EmployeeCode.ToString(), leave);

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

                        notif.EmployeeCode = leave.EmployeeCode;
                        notif.NotificationType = "Leave";
                        notif.NotificationDescription = "You have applied for leave from: " + leave.StartDate + " to: " + leave.EndDate;
                        notif.NotificationUrl = "Home/SearchLeaves";
                        notif.CreatedDate = DateTime.UtcNow;
                        notif.IsActive = true;

                        using var __connection = _context.CreateConnection();
                        __connection.Open();
                        await _connection.ExecuteAsync(NotificationQuery, notif);
                        __connection.Close();

                        if (leave.LeaveStatusId == 2)
                        {
                            await SendLeaveActionedEmail(leavedId.ToString());

                            var _NotificationQuery = @"INSERT INTO Notifications 
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

                            var notification = new Notifications();

                            notification.EmployeeCode = leave.EmployeeCode;
                            notification.NotificationType = "Leave";
                            notification.NotificationDescription = "Your leave is approved from: " + leave.StartDate + " to: " + leave.EndDate;
                            notification.NotificationUrl = "Home/SearchLeaves";
                            notification.CreatedDate = DateTime.UtcNow;
                            notification.IsActive = true;

                            using var notifconnection = _context.CreateConnection();
                            notifconnection.Open();
                            await notifconnection.ExecuteAsync(_NotificationQuery, notification);
                            notifconnection.Close();

                        }
                    }
                    else
                    {
                        var UpdateQuery = @"UPDATE Leaves
                                            SET 
                                                UserId = @UserId,
                                                EmployeeCode = @EmployeeCode,
                                                LeaveTypeId = @LeaveTypeId,
                                                LeaveTypeName = @LeaveTypeName,
                                                StartDate = @StartDate,
                                                EndDate = @EndDate,
                                                TotalDays = @TotalDays,
                                                LeaveStatusId = @LeaveStatusId,
                                                LeaveStatusName = @LeaveStatusName,
                                                LeaveReason = @LeaveReason,
                                                ModifiedById = @ModifiedById,
                                                ModifiedBy = @ModifiedBy,
                                                ModifiedDate = @ModifiedDate,
                                                IsActive = @IsActive
                                            WHERE LeaveId = @LeaveId;";

                        leave.StartDate = DateTime.ParseExact(leave.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        leave.EndDate = DateTime.ParseExact(leave.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                        var _StartDate = DateTime.ParseExact(leave.StartDate + " 00:00:00", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        var _EndDate = DateTime.ParseExact(leave.EndDate + " 23:59:59", "yyyy-MM-dd", CultureInfo.InvariantCulture);

                        var daysdiff = Convert.ToInt32((_EndDate - _StartDate).TotalDays);

                        var isAutoApproveQuery = @"Select * from LeaveTypes where LeaveTypeId = @LeaveTypeId and IsActive = 1";
                        using var _connection = _context.CreateConnection();
                        _connection.Open();
                        var leavetype = await _connection.QueryFirstOrDefaultAsync<LeaveTypes>(isAutoApproveQuery, leave);
                        _connection.Close();

                        if (leavetype != null && leavetype.AutoApprove == true)
                        {
                            leave.LeaveStatusId = 2;
                            leave.LeaveStatusName = "Approved";
                        }
                        else
                        {
                            leave.LeaveStatusId = 1;
                            leave.LeaveStatusName = "Pending";
                        }

                        leave.UserId = loggedinUserId;
                        leave.EmployeeCode = EmployeeCode;
                        leave.LeaveStatusId = 1;
                        leave.LeaveStatusName = "Pending";
                        leave.TotalDays = daysdiff;
                        leave.ModifiedById = loggedinUserId;
                        leave.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        leave.ModifiedDate = DateTime.UtcNow;
                        leave.IsActive = true;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(UpdateQuery, leave);
                        connection.Close();

                        await SendLeaveAddedEmail(leave.EmployeeCode.ToString(), leave);

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

        [HttpGet("GetLeaveById")]
        public async Task<ActionResult> GetLeaveById(string leaveId)
        {
            try
            {
                var sitequery = "Select * from Leaves Where LeaveId = @LeaveId";
                var paramas = new { LeaveId = leaveId };
                using var connection = _context.CreateConnection();
                connection.Open();
                var sitedata = await connection.QueryFirstOrDefaultAsync<Leaves>(sitequery, paramas);
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



        [HttpGet("GetLeavesByEmpCode")]
        public async Task<ActionResult> GetLeavesByEmpCode(string EmployeeCode)
        {
            try
            {
                var leavesquery = @"SELECT 
                                    LeaveId,
                                    LeaveTypeName,
                                    StartDate,
                                    EndDate,
                                    TotalDays,
                                    LeaveStatusId,
                                    LeaveStatusName,
                                    LeaveReason
                                FROM Leaves
                                WHERE 
                                    EmployeeCode = @EmployeeCode
                                    AND StartDate >= CAST(GETDATE() AS DATE)
                                    AND IsActive = 1
                                ORDER BY StartDate";
                var paramas = new { EmployeeCode = EmployeeCode };
                using var connection = _context.CreateConnection();
                connection.Open();
                var leavesdata = await connection.QueryAsync<Leaves>(leavesquery, paramas);
                var leavesList = leavesdata.ToList<Leaves>();
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    Leaves = leavesList
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
        [HttpPost("DeleteLeaveById")]
        public async Task<ActionResult> DeleteLeaveById(string leaveId)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var query = @"Update Leaves SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                   Where LeaveId = @LeaveId";
                var paramas = new { LeaveId = leaveId, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false };
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
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }

        [HttpPost("AprroveRejectLeave")]
        public async Task<ActionResult> AprroveRejectLeave(ApproveRejectLeaves approvereject)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");
                var EmployeeCode = HttpContext.Session.GetInt32("EmployeeCode");

                var leavequery = @"Update Leaves SET
                                   LeaveStatusId = @LeaveStatusId 
                                  ,LeaveStatusName = @LeaveStatusName 
                                  ,Comments = @Comments 
                                  ,ApprovedById = @ApprovedById 
                                  ,ApprovedBy = @ApprovedBy 
                                  ,ApprovedDate = @ApprovedDate 
                                   Where LeaveId = @LeaveId";

                approvereject.ApprovedById = loggedinUserId;
                approvereject.ApprovedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                approvereject.ApprovedDate = DateTime.UtcNow;
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(leavequery, approvereject);
                connection.Close();

                await SendLeaveActionedEmail(approvereject.LeaveId.ToString());

                var query = "Select * from Leaves Where LeaveId = @LeaveId";
                var paramas = new { LeaveId = approvereject.LeaveId };
                using var leaveconnection = _context.CreateConnection();
                leaveconnection.Open();
                var leavedata = await leaveconnection.QueryFirstOrDefaultAsync<Leaves>(query, paramas);
                leaveconnection.Close();


                var _NotificationQuery = @"INSERT INTO Notifications 
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

                var notification = new Notifications();

                notification.EmployeeCode = leavedata.EmployeeCode;
                notification.NotificationType = "Leave";
                notification.NotificationDescription = "Your leave is " + leavedata.LeaveStatusName + " from: " + leavedata.StartDate + " to: " + leavedata.EndDate;
                notification.NotificationUrl = "Home/SearchLeaves";
                notification.CreatedDate = DateTime.UtcNow;
                notification.IsActive = true;

                using var notifconnection = _context.CreateConnection();
                notifconnection.Open();
                await notifconnection.ExecuteAsync(_NotificationQuery, notification);
                notifconnection.Close();

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


        [HttpPost("SearchLeaves")]
        public async Task<IActionResult> SearchLeaves(LeavesFilter filters)
        {
            try
            {
                DataTableResponce dtr = new DataTableResponce();
                var tuple = await FilterLeavesData(filters);
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


        private async Task<Tuple<List<Leaves>, int>> FilterLeavesData(LeavesFilter search)
        {
            try
            {
                //var timezoneoffset = Convert.ToInt32(HttpContext.Current.Request.Cookies["timeZoneOffsetCookie"].Value);

                var whereClause = $" Where L.IsActive = @IsActive";

                search.IsActive = true;

                if (search.EmployeeCode != null && search.EmployeeCode.Count > 0)
                {
                    whereClause += "\n AND L.EmployeeCode IN (" + string.Join(", ", search.EmployeeCode) + ")";
                }

                if (!string.IsNullOrEmpty(search.StartDate))
                {
                    search.StartDate = DateOnly.ParseExact(search.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
                    whereClause += "\n AND L.StartDate >= @StartDate";
                }

                if (!string.IsNullOrEmpty(search.EndDate))
                {
                    search.EndDate = DateOnly.ParseExact(search.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
                    whereClause += "\n AND L.EndDate <= @EndDate";
                }

                if (search.LeaveTypeId != null)
                {
                    whereClause += "\n AND L.LeaveTypeId = @LeaveTypeId";
                }
                if (search.LeaveStatusId != null)
                {
                    whereClause += "\n AND L.LeaveStatusId = @LeaveStatusId";
                }

                var orderByClause = " ";

                if (string.IsNullOrEmpty(search.SortCol))
                {
                    orderByClause = "\n ORDER BY L.ModifiedDate DESC";

                }
                else
                {
                    if (search.sSortDir_0.Equals("asc"))
                    {
                        if (search.SortCol.Equals("SiteName"))
                        {
                            orderByClause = "\n ORDER BY L.SiteName asc";
                        }
                        else if (search.SortCol.Equals("TimeZone"))
                        {
                            orderByClause = "\n ORDER BY L.TimeZone asc";

                        }
                        else if (search.SortCol.Equals("CountryName"))
                        {
                            orderByClause = "\n ORDER BY L.CountryName asc";
                        }
                    }
                    if (search.sSortDir_0.Equals("desc"))
                    {

                        if (search.SortCol.Equals("SiteName"))
                        {
                            orderByClause = "\n ORDER BY L.SiteName desc";
                        }
                        else if (search.SortCol.Equals("TimeZone"))
                        {
                            orderByClause = "\n ORDER BY L.TimeZone desc";

                        }
                        else if (search.SortCol.Equals("CountryName"))
                        {
                            orderByClause = "\n ORDER BY L.CountryName desc";
                        }
                    }

                }

                var joins = @"";

                var sqlForCount = @"SELECT COUNT(L.LeaveId) FROM dbo.Leaves L "
                   + joins
                   + whereClause;


                using var connection = _context.CreateConnection();
                connection.Open();
                var totalCount = await connection.QuerySingleAsync<int>(sqlForCount, search);
                //connection.Close();

                var sql = @"SELECT 
                            L.LeaveId, 
                            L.UserId, 
                            L.EmployeeCode, 
                            L.EmployeeName, 
                            L.LeaveTypeId, 
                            L.LeaveTypeName, 
                            L.StartDate, 
                            L.EndDate, 
                            L.TotalDays, 
                            L.LeaveStatusId, 
                            L.LeaveStatusName, 
                            L.LeaveReason, 
                            L.ApprovedById, 
                            L.ApprovedBy, 
                            L.ApprovedDate, 
                            L.Comments,
                            L.CreatedById, 
                            L.CreatedBy, 
                            L.CreatedDate, 
                            L.ModifiedById, 
                            L.ModifiedBy, 
                            L.ModifiedDate, 
                            L.IsActive
                            FROM dbo.Leaves L"
                            + joins
                            + whereClause
                            + orderByClause
                            + "\n OFFSET @iDisplayStart ROWS FETCH NEXT @iDisplayLength ROWS ONLY;";
                var data = await connection.QueryAsync<Leaves>(sql, search);
                var LeavesData = data.ToList<Leaves>();
                //var data = db.Database.SqlQuery<Sites>(sql, sqlParams.ToArray()).ToList();
                return new Tuple<List<Leaves>, int>(LeavesData, totalCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet("GetLeaveTypes")]
        public async Task<ActionResult> GetLeaveTypes()
        {
            try
            {
                var departmentquery = @"SELECT *
                                        FROM LeaveTypes LT
                                        WHERE LT.IsActive = 1";

                using var connection = _context.CreateConnection();
                connection.Open();
                var leaveTypesData = await connection.QueryAsync<LeaveTypes>(departmentquery, null);
                var leaveTypesList = leaveTypesData.ToList<LeaveTypes>();

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    LeaveTypes = leaveTypesList
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

        [HttpPost("UpsertLeaveTypes")]
        public async Task<IActionResult> UpsertLeaveTypes(LeaveTypes leavetype)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");
                var EmployeeCode = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeCode"));


                if (leavetype != null)
                {
                    if (leavetype.LeaveTypeId == null)
                    {
                        var InsertQuery = @"INSERT INTO LeaveTypes (
                                                LeaveTypeName,
                                                LeaveTypeShortName,
                                                AutoApprove,
                                                CreatedById,
                                                CreatedBy,
                                                CreatedDate,
                                                ModifiedById,
                                                ModifiedBy,
                                                ModifiedDate,
                                                IsActive,
                                                IsActiveApproved
                                            )
                                            VALUES (
                                                @LeaveTypeName,
                                                @LeaveTypeShortName,
                                                @AutoApprove,
                                                @CreatedById,
                                                @CreatedBy,
                                                @CreatedDate,
                                                @ModifiedById,
                                                @ModifiedBy,
                                                @ModifiedDate,
                                                @IsActive,
                                                @IsActiveApproved
                                            );";

                        leavetype.CreatedById = loggedinUserId;
                        leavetype.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        leavetype.CreatedDate = DateTime.UtcNow;
                        leavetype.ModifiedById = loggedinUserId;
                        leavetype.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        leavetype.ModifiedDate = DateTime.UtcNow;
                        leavetype.IsActive = true;
                        leavetype.IsActiveApproved = false;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(InsertQuery, leavetype);
                        connection.Close();
                    }
                    else
                    {
                        var UpdateQuery = @"UPDATE LeaveTypes
                                            SET
                                                LeaveTypeName = @LeaveTypeName,
                                                LeaveTypeShortName = @LeaveTypeShortName,
                                                AutoApprove = @AutoApprove,
                                                ModifiedById = @ModifiedById,
                                                ModifiedBy = @ModifiedBy,
                                                ModifiedDate = @ModifiedDate
                                            WHERE
                                                LeaveTypeId = @LeaveTypeId;";

                        leavetype.ModifiedById = loggedinUserId;
                        leavetype.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        leavetype.ModifiedDate = DateTime.UtcNow;
                        leavetype.IsActive = true;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(UpdateQuery, leavetype);
                        connection.Close();
                    }
                }

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

        [HttpPost("UpsertLeavePolicy")]
        public async Task<IActionResult> UpsertLeavePolicy(LeavesPolicy policy)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");
                var EmployeeCode = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeCode"));


                if (policy != null)
                {
                    if (policy.Id == null)
                    {
                        var alreadyexistquery = @"Select * from LeavesPolicy where PolicyTypeId = @PolicyTypeId and IsActive = 1";


                        using var _connection = _context.CreateConnection();
                        _connection.Open();
                        var alreadyexists = await _connection.QueryFirstOrDefaultAsync<LeavesPolicy>(alreadyexistquery, policy);
                        _connection.Close();
                        if (alreadyexists != null)
                        {
                            return StatusCode(409, new
                            {
                                StatusCode = 409,
                                Message = "Leaves Policy against this Policy Type already exist."
                                //Message = "Site created successfully!",
                                //Data = new { Id = productId }
                            });

                        }
                        else
                        {

                            var InsertQuery = @"INSERT INTO LeavesPolicy (
                                                PolicyName,
                                                PolicyTypeId,
                                                PolicyType,
                                                PolicyDays,
                                                PolicyPeriod,
                                                ApplyDays,
                                                MovetoNextPeriod,
                                                CreatedById,
                                                CreatedBy,
                                                CreatedDate,
                                                ModifiedById,
                                                ModifiedBy,
                                                ModifiedDate,
                                                IsActive,
                                                IsActiveApproved
                                            )
                                            VALUES 
                                            (
                                                @PolicyName,
                                                @PolicyTypeId,
                                                @PolicyType,
                                                @PolicyDays,
                                                @PolicyPeriod,
                                                @ApplyDays,
                                                @MovetoNextPeriod,
                                                @CreatedById,
                                                @CreatedBy,
                                                @CreatedDate,
                                                @ModifiedById,
                                                @ModifiedBy,
                                                @ModifiedDate,
                                                @IsActive,
                                                @IsActiveApproved
                                            );";

                            policy.CreatedById = loggedinUserId;
                            policy.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                            policy.CreatedDate = DateTime.UtcNow;
                            policy.ModifiedById = loggedinUserId;
                            policy.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                            policy.ModifiedDate = DateTime.UtcNow;
                            policy.IsActive = true;
                            policy.IsActiveApproved = false;
                            using var connection = _context.CreateConnection();
                            connection.Open();
                            await connection.ExecuteAsync(InsertQuery, policy);
                            connection.Close();
                        }

                    }
                    else
                    {
                        var UpdateQuery = @"UPDATE LeavesPolicy
                                            SET
                                                PolicyName = @PolicyName,
                                                PolicyTypeId = @PolicyTypeId,
                                                PolicyType = @PolicyType,
                                                PolicyDays = @PolicyDays,
                                                PolicyPeriod = @PolicyPeriod,
                                                ApplyDays = @ApplyDays,
                                                MovetoNextPeriod = @MovetoNextPeriod,
                                                ModifiedById = @ModifiedById,
                                                ModifiedBy = @ModifiedBy,
                                                ModifiedDate = @ModifiedDate,
                                                IsActive = @IsActive
                                            WHERE Id = @Id;";

                        policy.ModifiedById = loggedinUserId;
                        policy.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                        policy.ModifiedDate = DateTime.UtcNow;
                        policy.IsActive = true;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(UpdateQuery, policy);
                        connection.Close();
                    }


                }

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


        [HttpPost("DeleteLeaveTypeById")]
        public async Task<ActionResult> DeleteLeaveTypeById(int leaveTypeId)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var sitequery = @"Update LeaveTypes SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                   Where LeaveTypeId = @leaveTypeId


                                   Update LeavesPolicy SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                   Where PolicyTypeId = @leaveTypeId";


                var paramas = new { leaveTypeId = leaveTypeId, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(sitequery, paramas);
                connection.Close();


                var query = "Select * from LeaveTypes where LeaveTypeId = @LeaveTypeId";
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var leaveTypeData = await connection.QueryFirstOrDefaultAsync<LeaveTypes>(query, paramas);
                _connection.Close();

                await SendDeletionEmail(leaveTypeData);

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

        [HttpPost("ApproveDeleteLeaveTypeById")]
        public async Task<ActionResult> ApproveDeleteLeaveTypeById(int leaveTypeId)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");


                var query = @"Select E.Email,E.FirstName, E.LastName from LeaveTypes LT 
                            JOIN USERS U ON U.UserId = LT.ModifiedById
                            JOIN Employee E ON E.EmployeeCode = U.EmployeeCode
                            Where LT.LeaveTypeId = @leaveTypeId";

                var _params = new { leaveTypeId = leaveTypeId };
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var employee = await _connection.QueryFirstOrDefaultAsync<Employee>(query, _params);
                _connection.Close();



                var sitequery = @"Update LeaveTypes SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                  ,IsActiveApproved = @IsActiveApproved 
                                   Where LeaveTypeId = @leaveTypeId


                                   Update LeavesPolicy SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                  ,IsActiveApproved = @IsActiveApproved 
                                   Where PolicyTypeId = @leaveTypeId";


                var paramas = new { leaveTypeId = leaveTypeId, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false, IsActiveApproved = true };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(sitequery, paramas);
                connection.Close();


                var _query = "Select * from LeaveTypes where leaveTypeId = @leaveTypeId";
                using var __connection = _context.CreateConnection();
                __connection.Open();
                var leaveTypedata = await connection.QueryFirstOrDefaultAsync<LeaveTypes>(_query, paramas);
                __connection.Close();

                await SendDeletionApprovalEmail(employee, leaveTypedata);



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



        [HttpPost("DeleteLeavePolicyById")]
        public async Task<ActionResult> DeleteLeavePolicyById(int policyId)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var policyquery = @"Update LeavesPolicy SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                   Where Id = @policyId";


                var paramas = new { policyId = policyId, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(policyquery, paramas);
                connection.Close();


                var query = "Select * from LeavesPolicy where Id = @policyId";
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var leavePolicyData = await connection.QueryFirstOrDefaultAsync<LeavesPolicy>(query, paramas);
                _connection.Close();

                await SendPolicyDeletionEmail(leavePolicyData);

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


        [HttpPost("ApproveDeleteLeavePolicyById")]
        public async Task<ActionResult> ApproveDeleteLeavePolicyById(int policyId)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var query = @"Select E.Email,E.FirstName, E.LastName from LeavesPolicy LP 
                            JOIN USERS U ON U.UserId = LP.ModifiedById
                            JOIN Employee E ON E.EmployeeCode = U.EmployeeCode
                            Where LP.Id = @policyId";

                var _params = new { policyId = policyId };
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var employee = await _connection.QueryFirstOrDefaultAsync<Employee>(query, _params);
                _connection.Close();

                var policyquery = @"Update LeavesPolicy SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                  ,IsActiveApproved = @IsActiveApproved 
                                   Where Id = @policyId";


                var paramas = new { policyId = policyId, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false, IsActiveApproved = true };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(policyquery, paramas);
                connection.Close();


                var _query = "Select * from LeavesPolicy where Id = @policyId";
                using var __connection = _context.CreateConnection();
                __connection.Open();
                var leavePolicydata = await connection.QueryFirstOrDefaultAsync<LeavesPolicy>(_query, paramas);
                __connection.Close();

                await SendPolicyDeletionApprovalEmail(employee, leavePolicydata);

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

        [HttpPost("SearchLeaveTypes")]
        public async Task<IActionResult> SearchLeaveTypes(LeaveTypesFilter filters)
        {
            try
            {
                DataTableResponce dtr = new DataTableResponce();
                var tuple = await FilterLeaveTypesData(filters);
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
        private async Task<Tuple<List<LeaveTypes>, int>> FilterLeaveTypesData(LeaveTypesFilter search)
        {
            try
            {
                //var timezoneoffset = Convert.ToInt32(HttpContext.Current.Request.Cookies["timeZoneOffsetCookie"].Value);

                var whereClause = $" Where (LT.IsActiveApproved = 0 OR LT.IsActiveApproved IS NULL)";

                search.IsActive = true;


                if (!string.IsNullOrEmpty(search.LeaveTypeName))
                {
                    whereClause += "\n AND LT.LeaveTypeName Like '%'+@LeaveTypeName+'%'";
                }

                if (search.AutoApprove != null)
                {
                    whereClause += "\n AND LT.AutoApprove = @AutoApprove";
                }

                var orderByClause = " ";

                if (string.IsNullOrEmpty(search.SortCol))
                {
                    orderByClause = "\n ORDER BY LT.ModifiedDate DESC";

                }
                else
                {
                    if (search.sSortDir_0.Equals("asc"))
                    {
                        if (search.SortCol.Equals("LeaveTypeName"))
                        {
                            orderByClause = "\n ORDER BY LT.LeaveTypeName asc";
                        }
                        else if (search.SortCol.Equals("AutoApprove"))
                        {
                            orderByClause = "\n ORDER BY LT.AutoApprove asc";

                        }
                    }
                    if (search.sSortDir_0.Equals("desc"))
                    {

                        if (search.SortCol.Equals("LeaveTypeName"))
                        {
                            orderByClause = "\n ORDER BY LT.LeaveTypeName desc";
                        }
                        else if (search.SortCol.Equals("AutoApprove"))
                        {
                            orderByClause = "\n ORDER BY LT.AutoApprove desc";
                        }

                    }

                }

                var joins = @"";

                var sqlForCount = @"SELECT COUNT(LT.LeaveTypeId) FROM dbo.LeaveTypes LT "
                   + joins
                   + whereClause;


                using var connection = _context.CreateConnection();
                connection.Open();
                var totalCount = await connection.QuerySingleAsync<int>(sqlForCount, search);
                //connection.Close();

                var sql = @"SELECT    
                                LT.LeaveTypeId,
                                LT.LeaveTypeName,
                                LT.LeaveTypeShortName,
                                LT.AutoApprove,
                                LT.CreatedById,
                                LT.CreatedBy,
                                LT.CreatedDate,
                                LT.ModifiedById,
                                LT.ModifiedBy,
                                LT.ModifiedDate,
                                LT.IsActive
                            FROM LeaveTypes LT"
                            + joins
                            + whereClause
                            + orderByClause
                            + "\n OFFSET @iDisplayStart ROWS FETCH NEXT @iDisplayLength ROWS ONLY;";
                var data = await connection.QueryAsync<LeaveTypes>(sql, search);
                var LeaveTypesData = data.ToList<LeaveTypes>();
                //var data = db.Database.SqlQuery<Sites>(sql, sqlParams.ToArray()).ToList();
                return new Tuple<List<LeaveTypes>, int>(LeaveTypesData, totalCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("SearchLeavesPolicy")]
        public async Task<IActionResult> SearchLeavesPolicy(LeavesPolicyFilter filters)
        {
            try
            {
                DataTableResponce dtr = new DataTableResponce();
                var tuple = await FilterLeavePolicyData(filters);
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
        private async Task<Tuple<List<LeavesPolicy>, int>> FilterLeavePolicyData(LeavesPolicyFilter search)
        {
            try
            {
                //var timezoneoffset = Convert.ToInt32(HttpContext.Current.Request.Cookies["timeZoneOffsetCookie"].Value);

                var whereClause = $" Where (LP.IsActiveApproved = 0 OR LP.IsActiveApproved IS NULL)";

                search.IsActive = true;


                if (!string.IsNullOrEmpty(search.PolicyName))
                {
                    whereClause += "\n AND LP.PolicyName like '%'+@PolicyName+'%'";
                }
                if (!string.IsNullOrEmpty(search.PolicyPeriod))
                {
                    whereClause += "\n AND LP.PolicyPeriod = @PolicyPeriod";
                }
                if (search.PolicyTypeId != null)
                {
                    whereClause += "\n AND LP.PolicyTypeId = @PolicyTypeId";
                }

                var orderByClause = " ";

                if (string.IsNullOrEmpty(search.SortCol))
                {
                    orderByClause = "\n ORDER BY LP.ModifiedDate DESC";

                }
                else
                {
                    if (search.sSortDir_0.Equals("asc"))
                    {
                        if (search.SortCol.Equals("PolicyName"))
                        {
                            orderByClause = "\n ORDER BY LP.PolicyName asc";
                        }
                        if (search.SortCol.Equals("PolicyType"))
                        {
                            orderByClause = "\n ORDER BY LP.PolicyType asc";
                        }
                        else if (search.SortCol.Equals("PolicyPeriod"))
                        {
                            orderByClause = "\n ORDER BY LP.PolicyPeriod asc";
                        }
                    }
                    if (search.sSortDir_0.Equals("desc"))
                    {

                        if (search.SortCol.Equals("PolicyName"))
                        {
                            orderByClause = "\n ORDER BY LP.PolicyName desc";
                        }
                        if (search.SortCol.Equals("PolicyType"))
                        {
                            orderByClause = "\n ORDER BY LP.PolicyType desc";
                        }
                        else if (search.SortCol.Equals("PolicyPeriod"))
                        {
                            orderByClause = "\n ORDER BY LP.PolicyPeriod desc";
                        }

                    }

                }

                var joins = @"";

                var sqlForCount = @"SELECT COUNT(LP.Id) FROM dbo.LeavesPolicy LP "
                   + joins
                   + whereClause;


                using var connection = _context.CreateConnection();
                connection.Open();
                var totalCount = await connection.QuerySingleAsync<int>(sqlForCount, search);
                //connection.Close();

                var sql = @"SELECT
                                LP.Id,
                                LP.PolicyName,
                                LP.PolicyTypeId,
                                LP.PolicyType,
                                LP.PolicyDays,
                                LP.PolicyPeriod,
                                LP.ApplyDays,
                                LP.MovetoNextPeriod,
                                LP.CreatedById,
                                LP.CreatedBy,
                                LP.CreatedDate,
                                LP.ModifiedById,
                                LP.ModifiedBy,
                                LP.ModifiedDate,
                                LP.IsActive
                            FROM LeavesPolicy LP"
                            + joins
                            + whereClause
                            + orderByClause
                            + "\n OFFSET @iDisplayStart ROWS FETCH NEXT @iDisplayLength ROWS ONLY;";
                var data = await connection.QueryAsync<LeavesPolicy>(sql, search);
                var LeavesPolicyData = data.ToList<LeavesPolicy>();
                //var data = db.Database.SqlQuery<Sites>(sql, sqlParams.ToArray()).ToList();
                return new Tuple<List<LeavesPolicy>, int>(LeavesPolicyData, totalCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task SendLeaveAddedEmail(string employeeCode, Leaves leave)
        {
            try
            {
                // Get employee email
                var emailQuery = @"SELECT * FROM Employee WHERE EmployeeCode = @EmployeeCode";
                using var connection = _context.CreateConnection();
                connection.Open();
                var employeeDetails = await connection.QueryFirstOrDefaultAsync<Employee>(emailQuery, new { EmployeeCode = employeeCode });
                connection.Close();

                if (employeeDetails != null)
                {
                    var subject = $"Leave Submission Request - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear {employeeDetails.FirstName + " " + employeeDetails.LastName},</p></br>
                            <p>Your have applied leave for below mentioned details.</p></br>
                            <p>Employee Name: {leave.EmployeeName}</p>
                            <p>Leave Type: {leave.LeaveTypeName}</p>
                            <p>From Date: {leave.StartDate}</p>
                            <p>To Date: {leave.EndDate}</p>
                            <p>Days: {leave.TotalDays}</p>
                            <p>Please access HR System at: <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a></p>
                            <p>Best regards,<br>Revital Health HR Management System</p><p><img src='cid:signatureImage' alt='Revital Health' style='height:60px; margin-top:10px;'/></p>
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

                // Get employee email
                var emailAdminQuery = @"SELECT E.FirstName,E.LastName,E.EMail FROM Employee E 
                                        JOIN USERS U ON U.EmployeeCode = E.EmployeeCode 
                                        JOIN Roles R ON R.RoleId = U.UserRoles  
                                        WHERE (R.RoleName like '%Admin%' OR R.RoleName like '%director%') AND U.IsActive = 1 AND E.Status = 1";
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
                if (employeeDetails != null)
                {
                    var subject = $"Leave Submission Request - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear All,</p></br>
                            <p>I would like to Submit leave with below mentioned details:</p></br>
                            <p>Employee Name: {leave.EmployeeName}</p>
                            <p>Leave Type: {leave.LeaveTypeName}</p>
                            <p>From Date: {leave.StartDate}</p>
                            <p>To Date: {leave.EndDate}</p>
                            <p>Days: {leave.TotalDays}</p>
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

        private async Task SendLeaveActionedEmail(string LeaveId)
        {
            try
            {
                var leavequery = "Select * from Leaves Where LeaveId = @LeaveId";
                var paramas = new { LeaveId = LeaveId };
                using var leaveconnection = _context.CreateConnection();
                leaveconnection.Open();
                var leavedata = await leaveconnection.QueryFirstOrDefaultAsync<Leaves>(leavequery, paramas);
                leaveconnection.Close();

                // Get employee email
                var emailQuery = @"SELECT * FROM Employee WHERE EmployeeCode = @EmployeeCode";
                using var connection = _context.CreateConnection();
                connection.Open();
                var employeeDetails = await connection.QueryFirstOrDefaultAsync<Employee>(emailQuery, new { EmployeeCode = leavedata.EmployeeCode });
                connection.Close();

                if (employeeDetails != null)
                {
                    var subject = $"Leave Submission Request - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear {employeeDetails.FirstName + " " + employeeDetails.LastName},</p></br>
                            <p>Your leave is {leavedata.LeaveStatusName} for below mentioned details.</p></br>
                            <p>Employee Name: {leavedata.EmployeeName}</p>
                            <p>Leave Type: {leavedata.LeaveTypeName}</p>
                            <p>From Date: {leavedata.StartDate}</p>
                            <p>To Date: {leavedata.EndDate}</p>
                            <p>Days: {leavedata.TotalDays}</p>
                            <p>Please access HR System at: <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a></p>
                            <p>Best regards,<br>Revital Health HR Management System</p><p><img src='cid:signatureImage' alt='Revital Health' style='height:60px; margin-top:10px;'/></p>
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

                // Get employee email
                var emailAdminQuery = @"SELECT E.FirstName,E.LastName,E.EMail FROM Employee E 
                                        JOIN USERS U ON U.EmployeeCode = E.EmployeeCode 
                                        JOIN Roles R ON R.RoleId = U.UserRoles  
                                        WHERE (R.RoleName like '%Admin%' OR R.RoleName like '%director%') AND U.IsActive = 1 AND E.Status = 1";
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
                if (employeeDetails != null)
                {
                    var subject = $"Leave Submission Request - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear All,</p></br>
                            <p>Leave of {leavedata.EmployeeName} is {leavedata.LeaveStatusName} with below mentioned details:</p></br>
                            <p>Employee Name: {leavedata.EmployeeName}</p>
                            <p>Leave Type: {leavedata.LeaveTypeName}</p>
                            <p>From Date: {leavedata.StartDate}</p>
                            <p>To Date: {leavedata.EndDate}</p>
                            <p>Days: {leavedata.TotalDays}</p>
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

        private async Task SendDeletionEmail(LeaveTypes leaveType)
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
                    var subject = $"Leave Type Deletion Request - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear All,</p></br>
                            <p>{leaveType.ModifiedBy} has requested to delete the leave type named {leaveType.LeaveTypeName}</p></br>
                            <p>Access the HRMS at : <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a> and Navigate to Leaves Policy Management to approve the deletion request.</p></br></br>
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

        private async Task SendPolicyDeletionEmail(LeavesPolicy leavePolicy)
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
                    var subject = $"Leave Policy Deletion Request - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear All,</p></br>
                            <p>{leavePolicy.ModifiedBy} has requested to delete the leave policy named {leavePolicy.PolicyName}</p></br>
                            <p>Access the HRMS at : <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a> and Navigate to Leaves Policy Management to approve the deletion request.</p></br></br>
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
        private async Task SendDeletionApprovalEmail(Employee employee, LeaveTypes leaveType)
        {
            try
            {

                if (employee != null)
                {
                    var subject = $"Leave Type Deletion Request Approved - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear {employee.FirstName + " " + employee.LastName},</p></br>
                            <p>{leaveType.ModifiedBy} has approved the request to delete the leave type named {leaveType.LeaveTypeName} and it is deleted successfully from the system.</p></br>
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
        private async Task SendPolicyDeletionApprovalEmail(Employee employee, LeavesPolicy leavePolicy)
        {
            try
            {

                if (employee != null)
                {
                    var subject = $"Leave Policy Deletion Request Approved - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear {employee.FirstName + " " + employee.LastName},</p></br>
                            <p>{leavePolicy.ModifiedBy} has approved the request to delete the leave policy named {leavePolicy.PolicyName} and it is deleted successfully from the system.</p></br>
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
