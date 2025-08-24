using Dapper;
using HrManagement.Data;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;

namespace HrManagement.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleManagementAPIController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly Email _email;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public ScheduleManagementAPIController(DataContext context, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, IWebHostEnvironment env)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _env = env;
            _email = new Email(configuration, _env);
        }

        [HttpPost("UpsertSchedule")]
        public async Task<IActionResult> UpsertSchedule(EmployeeSchedule schedule)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                if (schedule != null)
                {
                    var ConflictQuery = @"SELECT s.*
                                        FROM EmployeeSchedule s
                                        CROSS APPLY OPENJSON(s.OnDays) WITH (DayValue NVARCHAR(2) '$') AS scheduleDay
                                        CROSS APPLY OPENJSON(@OnDays) WITH (DayValue NVARCHAR(2) '$') AS newDay
                                        WHERE 
                                            s.EmployeeCode = @EmployeeCode
                                            AND s.IsActive = 1
                                            AND s.StartDate <= @EndDate
                                            AND s.EndDate >= @StartDate
                                            AND s.StartTime < @EndTime
                                            AND s.EndTime > @StartTime
                                            AND scheduleDay.DayValue = newDay.DayValue";
                    using var _connection = _context.CreateConnection();
                    _connection.Open();
                    var presentSchedule = await _connection.QueryAsync<EmployeeSchedule>(ConflictQuery, schedule);
                    var presentScheduleList = presentSchedule.ToList<EmployeeSchedule>();
                    _connection.Close();

                    if (presentScheduleList != null && presentScheduleList.Count > 0 && schedule.Id == null)
                    {
                        return StatusCode(409, new
                        {
                            StatusCode = 409,
                            Message = "Schedule conflict found for the selected employee."
                        });
                    }
                    else
                    {
                        if (schedule.Id == null)
                        {
                            var InsertQuery = @"INSERT INTO EmployeeSchedule  
                                            (
                                                EmployeeCode
                                               ,EmployeeName
                                               ,SiteId
                                               ,SiteName
                                               ,StartDate
                                               ,EndDate
                                               ,StartTime
                                               ,EndTime
                                               ,OnDays
                                               ,OffDays
                                               ,CreatedById
                                               ,CreatedBy
                                               ,CreatedDate
                                               ,ModifiedById
                                               ,ModifiedBy
                                               ,ModifiedDate
                                               ,IsActive
                                               ,IsActiveApproved
                                            )
                                            VALUES 
                                            (
                                                @EmployeeCode    
                                               ,@EmployeeName
                                               ,@SiteId
                                               ,@SiteName
                                               ,@StartDate
                                               ,@EndDate
                                               ,@StartTime
                                               ,@EndTime
                                               ,@OnDays
                                               ,@OffDays
                                               ,@CreatedById
                                               ,@CreatedBy
                                               ,@CreatedDate
                                               ,@ModifiedById
                                               ,@ModifiedBy
                                               ,@ModifiedDate
                                               ,@IsActive
                                               ,@IsActiveApproved
                                            );

                                            SELECT CAST(SCOPE_IDENTITY() as int);";

                            schedule.CreatedById = loggedinUserId;
                            schedule.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                            schedule.CreatedDate = DateTime.UtcNow;
                            schedule.ModifiedById = loggedinUserId;
                            schedule.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                            schedule.ModifiedDate = DateTime.UtcNow;
                            schedule.IsActive = true;
                            schedule.IsActiveApproved = false;
                            using var connection = _context.CreateConnection();
                            connection.Open();
                            var scheduleId = await connection.ExecuteScalarAsync<int>(InsertQuery, schedule);
                            connection.Close();

                            // Add notification and send email for new schedule
                            await AddScheduleNotification(schedule, "created", loggedinUserId);
                            await SendScheduleEmail(schedule, "created");
                        }
                        else
                        {

                            var _ConflictQuery = @"SELECT s.*
                                        FROM EmployeeSchedule s
                                        CROSS APPLY OPENJSON(s.OnDays) WITH (DayValue NVARCHAR(2) '$') AS scheduleDay
                                        CROSS APPLY OPENJSON(@OnDays) WITH (DayValue NVARCHAR(2) '$') AS newDay
                                        WHERE 
                                            s.EmployeeCode = @EmployeeCode
                                            AND s.Id NOT IN (@Id)
                                            AND s.IsActive = 1
                                            AND s.StartDate <= @EndDate
                                            AND s.EndDate >= @StartDate
                                            AND s.StartTime < @EndTime
                                            AND s.EndTime > @StartTime
                                            AND scheduleDay.DayValue = newDay.DayValue";
                            using var __connection = _context.CreateConnection();
                            __connection.Open();
                            var _presentSchedule = await __connection.QueryAsync<EmployeeSchedule>(_ConflictQuery, schedule);
                            var _presentScheduleList = presentSchedule.ToList<EmployeeSchedule>();
                            __connection.Close();

                            if (_presentScheduleList != null && _presentScheduleList.Count > 0)
                            {
                                return StatusCode(409, new
                                {
                                    StatusCode = 409,
                                    Message = "Schedule conflict found for the selected employee."
                                });
                            }
                            else
                            {
                                var UpdateQuery = @"UPDATE EmployeeSchedule
                                            SET 
                                                EmployeeCode = @EmployeeCode
                                               ,EmployeeName = @EmployeeName
                                               ,SiteId = @SiteId
                                               ,SiteName = @SiteName
                                               ,StartDate = @StartDate
                                               ,EndDate = @EndDate
                                               ,StartTime = @StartTime
                                               ,EndTime = @EndTime
                                               ,OnDays = @OnDays
                                               ,OffDays = @OffDays
                                               ,ModifiedById = @ModifiedById
                                               ,ModifiedBy = @ModifiedBy
                                               ,ModifiedDate = @ModifiedDate
                                            WHERE Id = @Id";

                                schedule.ModifiedById = loggedinUserId;
                                schedule.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                                schedule.ModifiedDate = DateTime.UtcNow;
                                using var connection = _context.CreateConnection();
                                connection.Open();
                                await connection.ExecuteAsync(UpdateQuery, schedule);
                                connection.Close();

                                // Add notification and send email for updated schedule
                                await AddScheduleNotification(schedule, "updated", loggedinUserId);
                                await SendScheduleEmail(schedule, "updated");
                            }
                        }
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

        private async Task AddScheduleNotification(EmployeeSchedule schedule, string action, int loggedinUserId)
        {
            try
            {
                var notificationQuery = @"INSERT INTO Notifications
                                        (NotificationType, NotificationDescription, NotificationUrl, EmployeeCode, CreatedDate, IsActive)
                                        VALUES
                                        (@NotificationType, @NotificationDescription, @NotificationUrl, @EmployeeCode, @CreatedDate, @IsActive)";

                var notification = new
                {
                    NotificationType = "Schedule",
                    NotificationDescription = $"A schedule has been {action} for {schedule.EmployeeName} at {schedule.SiteName} from {schedule.StartDate?.ToString("MMM dd, yyyy")} to {schedule.EndDate?.ToString("MMM dd, yyyy")}",
                    NotificationUrl = $"Home/MySchedule",
                    EmployeeCode = schedule.EmployeeCode,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                };

                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(notificationQuery, notification);
                connection.Close();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
            }
        }

        private async Task SendScheduleEmail(EmployeeSchedule schedule, string action)
        {
            try
            {
                // Get employee email
                var emailQuery = @"SELECT Email FROM Employee WHERE EmployeeCode = @EmployeeCode";
                using var connection = _context.CreateConnection();
                connection.Open();
                var employeeEmail = await connection.QueryFirstOrDefaultAsync<string>(emailQuery, new { schedule.EmployeeCode });
                connection.Close();

                if (!string.IsNullOrEmpty(employeeEmail))
                {
                    var subject = $"Schedule {action} - HR Management System";
                    var body = $@"
                        <html>
                        <body>
                            <h2>Schedule {action}</h2>
                            <p>Dear {schedule.EmployeeName},</p>
                            <p>Your schedule has been {action} with the following details:</p>
                            <ul>
                                <li>Site: {schedule.SiteName}</li>
                                <li>Start Date: {schedule.StartDate?.ToString("MMM dd, yyyy")}</li>
                                <li>End Date: {schedule.EndDate?.ToString("MMM dd, yyyy")}</li>
                                <li>Start Time: {schedule.StartTime}</li>
                                <li>End Time: {schedule.EndTime}</li>
                            </ul>
                            <p>Please log in to the HR Management System to view your updated schedule. <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a></p>
                            <p>Best regards,<br>Revital Health HR Management System</p><p><img src='cid:signatureImage' alt='Revital Health' style='height:60px; margin-top:10px;' /></p>
                        </body>
                        </html>";

                    await _email.SendEmailAsync(
                        from: "info@hrms.chestermerephysio.ca",
                        toname: schedule.EmployeeName,
                        to: employeeEmail,
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

        [HttpPost("GetManageScheduleData")]
        public async Task<IActionResult> GetManageScheduleData(ManageScheduleFilters filters)
        {
            try
            {
                DataTableResponce dtr = new DataTableResponce();
                var tuple = await FilterManageSchduleData(filters);
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



        private async Task<Tuple<List<EmployeeSchedule>, int>> FilterManageSchduleData(ManageScheduleFilters search)
        {
            //var timezoneoffset = Convert.ToInt32(HttpContext.Current.Request.Cookies["timeZoneOffsetCookie"].Value);
            try
            {
                var sqlParams = new List<SqlParameter>();
                var sqlParams1 = new List<SqlParameter>();

                var whereClause = $" Where ES.IsActiveApproved = 0";

                if (search.EmployeeCode != null && search.EmployeeCode.Count > 0)
                {
                    whereClause += "\n AND ES.EmployeeCode IN (" + string.Join(", ", search.EmployeeCode) + ")  \r\n";
                    sqlParams.Add(new SqlParameter("@EmployeeCode", search.EmployeeCode));
                    sqlParams1.Add(new SqlParameter("@EmployeeCode", search.EmployeeCode));
                }

                if (search.StartDate.HasValue)
                {
                    var startDate = DateTime.ParseExact(search.StartDate.Value.ToString("yyyy-MM-dd") + " 00:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    search.StartDate = startDate;
                    whereClause += "\n AND ( @StartDate BETWEEN ES.StartDate AND ES.EndDate \r\n";
                    sqlParams.Add(new SqlParameter("@StartDate", search.StartDate));
                    sqlParams1.Add(new SqlParameter("@StartDate", search.StartDate));
                }
                if (search.EndDate.HasValue)
                {
                    var endDate = DateTime.ParseExact(search.EndDate.Value.ToString("yyyy-MM-dd") + " 23:59:59", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    search.EndDate = endDate;
                    whereClause += "\n     OR @EndDate BETWEEN ES.StartDate AND ES.EndDate \r\n";
                    whereClause += "\n     OR ES.StartDate >= @StartDate AND ES.EndDate <= @EndDate)\r\n";
                    sqlParams.Add(new SqlParameter("@EndDate", search.EndDate));
                    sqlParams1.Add(new SqlParameter("@EndDate", search.EndDate));
                }


                var orderByClause = " ";

                if (string.IsNullOrEmpty(search.SortCol))
                {
                    orderByClause = "\n ORDER BY ES.ModifiedDate DESC";

                }
                else
                {
                    if (search.sSortDir_0.Equals("asc"))
                    {
                        if (search.SortCol.Equals("EmployeeCode"))
                        {
                            orderByClause = "\n ORDER BY ES.EmployeeCode asc";
                        }
                        else if (search.SortCol.Equals("EmployeeName"))
                        {
                            orderByClause = "\n ORDER BY ES.EmployeeName asc";

                        }
                        else if (search.SortCol.Equals("StartDate"))
                        {
                            orderByClause = "\n ORDER BY ES.StartDate asc";
                        }
                        else if (search.SortCol.Equals("EndDate"))
                        {
                            orderByClause = "\n ORDER BY ES.EndDate asc";
                        }
                    }
                    if (search.sSortDir_0.Equals("desc"))
                    {
                        if (search.SortCol.Equals("EmployeeCode"))
                        {
                            orderByClause = "\n ORDER BY ES.EmployeeCode desc";
                        }
                        else if (search.SortCol.Equals("EmployeeName"))
                        {
                            orderByClause = "\n ORDER BY ES.EmployeeName desc";

                        }
                        else if (search.SortCol.Equals("StartDate"))
                        {
                            orderByClause = "\n ORDER BY ES.StartDate desc";
                        }
                        else if (search.SortCol.Equals("EndDate"))
                        {
                            orderByClause = "\n ORDER BY ES.EndDate desc";
                        }
                    }

                }

                var joins = @"";

                var sqlForCount = @"SELECT COUNT(ES.Id) FROM dbo.EmployeeSchedule ES "
                   + joins
                   + whereClause;


                using var connection = _context.CreateConnection();
                connection.Open();
                var totalCount = await connection.QuerySingleAsync<int>(sqlForCount, search);
                //connection.Close();

                var sql = @"SELECT 
	                         ES.Id
                            ,ES.EmployeeCode
                            ,ES.EmployeeName
                            ,ES.SiteId
                            ,ES.SiteName
                            ,ES.StartDate
                            ,ES.EndDate
                            ,ES.StartTime
                            ,ES.EndTime
                            ,ES.OnDays
                            ,ES.OffDays
                            ,ES.CreatedById
                            ,ES.CreatedBy
                            ,ES.CreatedDate
                            ,ES.ModifiedById
                            ,ES.ModifiedBy
                            ,ES.ModifiedDate
                            ,ES.IsActive
                            ,ES.IsActiveApproved
                            FROM dbo.EmployeeSchedule ES"
                            + joins
                            + whereClause
                            + orderByClause
                            + "\n OFFSET @iDisplayStart ROWS FETCH NEXT @iDisplayLength ROWS ONLY;";
                var data = await connection.QueryAsync<EmployeeSchedule>(sql, search);
                var EmployeeScheduleList = data.ToList<EmployeeSchedule>();
                //var data = db.Database.SqlQuery<Sites>(sql, sqlParams.ToArray()).ToList();
                return new Tuple<List<EmployeeSchedule>, int>(EmployeeScheduleList, totalCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpPost("GetSchedule")]
        public async Task<IActionResult> GetSchedule(ScheduleFilters filters)
        {
            try
            {
                var schedule = await FilterSchduleData(filters);

                var scheduleRoster = await CreateScheduleRoster(schedule);
                //return Ok(scheduleRoster);
                return StatusCode(200, new
                {
                    StatusCode = 200,
                    Data = scheduleRoster
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


        private async Task<List<EmployeeSchedule>?> FilterSchduleData(ScheduleFilters search)
        {
            //var timezoneoffset = Convert.ToInt32(HttpContext.Current.Request.Cookies["timeZoneOffsetCookie"].Value);

            try
            {
                var sqlParams = new List<SqlParameter>();
                var sqlParams1 = new List<SqlParameter>();

                var whereClause = $" Where Es.IsActiveApproved = 0";


                if (search.EmployeeCode != null && search.EmployeeCode != 0)
                {
                    whereClause += "\n AND ES.EmployeeCode = @EmployeeCode  \r\n";
                    sqlParams.Add(new SqlParameter("@EmployeeCode", search.EmployeeCode));
                    sqlParams1.Add(new SqlParameter("@EmployeeCode", search.EmployeeCode));
                }
                if (search.StartDate.HasValue)
                {
                    var startDate = DateTime.ParseExact(search.StartDate.Value.ToString("YYYY-MM-DD") + " 00:00:00", "YYYY-MM-DD HH:mm:ss", CultureInfo.InvariantCulture);
                    search.StartDate = startDate;
                    whereClause += "\n AND ( @StartDate BETWEEN ES.StartDate AND ES.EndDate \r\n";
                    sqlParams.Add(new SqlParameter("@StartDate", search.StartDate));
                    sqlParams1.Add(new SqlParameter("@StartDate", search.StartDate));
                }
                if (search.EndDate.HasValue)
                {
                    var endDate = DateTime.ParseExact(search.EndDate.Value.ToString("YYYY-MM-DD") + " 23:59:59", "YYYY-MM-DD HH:mm:ss", CultureInfo.InvariantCulture);
                    search.EndDate = endDate;
                    whereClause += "\n     OR @EndDate BETWEEN ES.StartDate AND ES.EndDate \r\n";
                    whereClause += "\n     OR ES.StartDate >= @StartDate AND ES.EndDate <= @EndDate)\r\n";
                    sqlParams.Add(new SqlParameter("@EndDate", search.EndDate));
                    sqlParams1.Add(new SqlParameter("@EndDate", search.EndDate));
                }


                var orderByClause = " ";
                var joins = @"";


                using var connection = _context.CreateConnection();
                connection.Open();

                var sql = @"SELECT * FROM dbo.EmployeeSchedule ES"
                            + joins
                            + whereClause;
                var data = await connection.QueryAsync<EmployeeSchedule>(sql, search);
                var EmployeeScheduleList = data.ToList<EmployeeSchedule>();
                //var data = db.Database.SqlQuery<Sites>(sql, sqlParams.ToArray()).ToList();
                return new List<EmployeeSchedule>(EmployeeScheduleList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("GetSupervisorsSchedule")]
        public async Task<IActionResult> GetSupervisorsSchedule(ScheduleFilters filters)
        {
            try
            {
                var schedule = await FilterSupervisorsSchduleData(filters);

                var scheduleRoster = await CreateScheduleRoster(schedule);
                //return Ok(scheduleRoster);
                return StatusCode(200, new
                {
                    StatusCode = 200,
                    Data = scheduleRoster
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


        private async Task<List<EmployeeSchedule>?> FilterSupervisorsSchduleData(ScheduleFilters search)
        {
            //var timezoneoffset = Convert.ToInt32(HttpContext.Current.Request.Cookies["timeZoneOffsetCookie"].Value);

            try
            {
                var sqlParams = new List<SqlParameter>();
                var sqlParams1 = new List<SqlParameter>();

                var whereClause = $" Where ES.IsActiveApproved = 0";


                if (search.EmployeeCode != null && search.EmployeeCode != 0 && search.SiteId != null)
                {
                    whereClause += "\n AND (ES.EmployeeCode = @EmployeeCode OR E.SiteId = @SiteId)  \r\n";
                    sqlParams.Add(new SqlParameter("@EmployeeCode", search.EmployeeCode));
                    sqlParams1.Add(new SqlParameter("@EmployeeCode", search.EmployeeCode));
                    sqlParams.Add(new SqlParameter("@SiteId", search.SiteId));
                    sqlParams1.Add(new SqlParameter("@SiteId", search.SiteId));
                }
                if (search.EmployeeCode != null && search.EmployeeCode != 0 && search.SiteId == null)
                {
                    whereClause += "\n AND ES.EmployeeCode = @EmployeeCode  \r\n";
                    sqlParams.Add(new SqlParameter("@EmployeeCode", search.EmployeeCode));
                    sqlParams1.Add(new SqlParameter("@EmployeeCode", search.EmployeeCode));
                }

                if ((search.EmployeeCode == null || search.EmployeeCode == 0) && search.SiteId != null)
                {
                    whereClause += "\n AND E.SiteId = @SiteId  \r\n";
                    sqlParams.Add(new SqlParameter("@SiteId", search.SiteId));
                    sqlParams1.Add(new SqlParameter("@SiteId", search.SiteId));
                }

                if (search.StartDate.HasValue)
                {
                    var startDate = DateTime.ParseExact(search.StartDate.Value.ToString("YYYY-MM-DD") + " 00:00:00", "YYYY-MM-DD HH:mm:ss", CultureInfo.InvariantCulture);
                    search.StartDate = startDate;
                    whereClause += "\n AND ( @StartDate BETWEEN ES.StartDate AND ES.EndDate \r\n";
                    sqlParams.Add(new SqlParameter("@StartDate", search.StartDate));
                    sqlParams1.Add(new SqlParameter("@StartDate", search.StartDate));
                }
                if (search.EndDate.HasValue)
                {
                    var endDate = DateTime.ParseExact(search.EndDate.Value.ToString("YYYY-MM-DD") + " 23:59:59", "YYYY-MM-DD HH:mm:ss", CultureInfo.InvariantCulture);
                    search.EndDate = endDate;
                    whereClause += "\n     OR @EndDate BETWEEN ES.StartDate AND ES.EndDate \r\n";
                    whereClause += "\n     OR ES.StartDate >= @StartDate AND ES.EndDate <= @EndDate)\r\n";
                    sqlParams.Add(new SqlParameter("@EndDate", search.EndDate));
                    sqlParams1.Add(new SqlParameter("@EndDate", search.EndDate));
                }


                var orderByClause = " ";
                var joins = @" JOIN Employee E ON E.EmployeeCode = Es.EmployeeCode";


                using var connection = _context.CreateConnection();
                connection.Open();

                var sql = @"SELECT 
                            ES.Id
                            ,ES.EmployeeCode
                            ,ES.EmployeeName
                            ,ES.SiteId
                            ,ES.SiteName
                            ,ES.StartDate
                            ,ES.EndDate
                            ,ES.StartTime
                            ,ES.EndTime
                            ,ES.OnDays
                            ,ES.OffDays
                            ,ES.CreatedById
                            ,ES.CreatedBy
                            ,ES.CreatedDate
                            ,ES.ModifiedById
                            ,ES.ModifiedBy
                            ,ES.ModifiedDate
                            ,ES.IsActive
                            ,ES.IsActiveApproved
                            FROM dbo.EmployeeSchedule ES"
                            + joins
                            + whereClause;
                var data = await connection.QueryAsync<EmployeeSchedule>(sql, search);
                var EmployeeScheduleList = data.ToList<EmployeeSchedule>();
                //var data = db.Database.SqlQuery<Sites>(sql, sqlParams.ToArray()).ToList();
                return new List<EmployeeSchedule>(EmployeeScheduleList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private async Task<List<EmployeeScheduleRoster>> CreateScheduleRoster(List<EmployeeSchedule> employeeSchedules)
        {
            try
            {
                var rosters = new List<EmployeeScheduleRoster>();

                foreach (var schedule in employeeSchedules)
                {
                    var onDays = JsonConvert.DeserializeObject<List<int>>(schedule.OnDays);
                    var offDays = JsonConvert.DeserializeObject<List<int>>(schedule.OffDays);
                    DateTime currentDate = Convert.ToDateTime(schedule.StartDate);
                    while (currentDate <= schedule.EndDate)
                    {
                        int dayOfWeek = (int)currentDate.DayOfWeek;
                        int adjustedDay = dayOfWeek == 0 ? 7 : dayOfWeek;

                        if (!offDays.Contains(adjustedDay)) // Skip Off Days
                        {
                            var roster = new EmployeeScheduleRoster();
                            roster.RosterId = Guid.NewGuid().ToString();
                            roster.EmployeeCode = schedule.EmployeeCode;
                            roster.EmployeeName = schedule.EmployeeName;
                            roster.SiteId = schedule.SiteId;
                            roster.SiteName = schedule.SiteName;
                            roster.StartDate = DateTime.Parse($"{currentDate:yyyy-MM-dd} {schedule.StartTime}");
                            roster.EndDate = DateTime.Parse($"{currentDate:yyyy-MM-dd} {schedule.EndTime}");
                            rosters.Add(roster);
                        }

                        currentDate = currentDate.AddDays(1); // Move to next day
                    }
                }

                return rosters;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost("DeleteScheduleById")]
        public async Task<ActionResult> DeleteScheduleById(string Id)
        {
            try
            {

                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var deletequery = @"Update EmployeeSchedule SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                   Where Id = @Id";
                var paramas = new { Id = Id, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(deletequery, paramas);
                connection.Close();


                var query = "Select * from EmployeeSchedule where Id = @Id";
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var empschduleData = await connection.QueryFirstOrDefaultAsync<EmployeeSchedule>(query, paramas);
                _connection.Close();

                await SendDeletionEmail(empschduleData);


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

        [HttpPost("ApproveDeleteScheduleById")]
        public async Task<ActionResult> ApproveDeleteScheduleById(string Id)
        {
            try
            {

                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var _query = @"Select E.Email,E.FirstName, E.LastName from EmployeeSchedule ES 
                            JOIN USERS U ON U.UserId = ES.ModifiedById
                            JOIN Employee E ON E.EmployeeCode = U.EmployeeCode
                            Where ES.Id = @Id";

                var _params = new { Id = Id };
                using var _connection = _context.CreateConnection();
                _connection.Open();
                var employee = await _connection.QueryFirstOrDefaultAsync<Employee>(_query, _params);
                _connection.Close();


                var deletequery = @"Update EmployeeSchedule SET
                                   ModifiedById = @ModifiedById 
                                  ,ModifiedBy = @ModifiedBy 
                                  ,ModifiedDate = @ModifiedDate 
                                  ,IsActive = @IsActive 
                                  ,IsActiveApproved = @IsActiveApproved 
                                   Where Id = @Id";
                var paramas = new { Id = Id, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false, IsActiveApproved = true };
                using var connection = _context.CreateConnection();
                connection.Open();
                await connection.ExecuteAsync(deletequery, paramas);
                connection.Close();


                var query = "Select * from EmployeeSchedule where Id = @Id";
                using var __connection = _context.CreateConnection();
                __connection.Open();
                var empschduleData = await connection.QueryFirstOrDefaultAsync<EmployeeSchedule>(query, paramas);
                __connection.Close();

                await SendDeletionApprovalEmail(employee,empschduleData);


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

        [HttpGet("GetScheduleById")]
        public async Task<ActionResult> GetScheduleById(string Id)
        {
            try
            {

                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var deletequery = @"Select * from EmployeeSchedule 
                                   Where Id = @Id and IsActive = 1";
                var paramas = new { Id = Id };
                using var connection = _context.CreateConnection();
                connection.Open();
                var Schedule = await connection.QueryFirstOrDefaultAsync<EmployeeSchedule>(deletequery, paramas);
                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    Schedule = Schedule
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


        private async Task SendDeletionEmail(EmployeeSchedule schedule)
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
                    var subject = $"Schedule Deletion Request - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear All,</p></br>
                            <p>{schedule.ModifiedBy} has requested to delete the schedule of employee named {schedule.EmployeeName} with following details</p></br>
                            <ul>
                                <li>Site: {schedule.SiteName}</li>
                                <li>Start Date: {schedule.StartDate?.ToString("MMM dd, yyyy")}</li>
                                <li>End Date: {schedule.EndDate?.ToString("MMM dd, yyyy")}</li>
                                <li>Start Time: {schedule.StartTime}</li>
                                <li>End Time: {schedule.EndTime}</li>
                            </ul>
                            <p>Access the HRMS at : <a href='https://hrms.chestermerephysio.ca/' target='_blank'>hrms.chestermerephysio.ca</a> and Navigate to Manage Schedule to approve the deletion request.</p></br></br>
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

        private async Task SendDeletionApprovalEmail(Employee employee, EmployeeSchedule schedule)
        {
            try
            {

                if (employee != null)
                {
                    var subject = $"Schedule Deletion Request Approved - Revital Health HRMS";
                    var body = $@"
                        <html>
                        <body>
                            <p>Dear {employee.FirstName + " " + employee.LastName},</p></br>
                            <p>{schedule.ModifiedBy} has approved the request to delete the schedule of employee named {schedule.EmployeeName} with the following details, and it is deleted successfully from the system.</p></br>
                            <ul>
                                <li>Site: {schedule.SiteName}</li>
                                <li>Start Date: {schedule.StartDate?.ToString("MMM dd, yyyy")}</li>
                                <li>End Date: {schedule.EndDate?.ToString("MMM dd, yyyy")}</li>
                                <li>Start Time: {schedule.StartTime}</li>
                                <li>End Time: {schedule.EndTime}</li>
                            </ul>
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
