using Dapper;
using DocumentFormat.OpenXml.Wordprocessing;
using HrManagement.Controllers;
using HrManagement.Data;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class HolidaysAPIController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly Email _email;
        public HolidaysAPIController(DataContext context, Email email)
        {
            _context = context;
            _email = email;
        }

        [HttpPost("UpsertHoliday")]
        public async Task<IActionResult> UpsertHoliday(Holiday holiday)
        {
            try
            {
                // get logged‑in user info from session
                var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var firstName = HttpContext.Session.GetString("FirstName") ?? "";
                var lastName = HttpContext.Session.GetString("LastName") ?? "";
                var fullName = $"{firstName} {lastName}".Trim();
                var now = DateTime.UtcNow;

                // set audit fields
                holiday.ModifiedById = userId;
                holiday.ModifiedBy = fullName;
                holiday.ModifiedDate = now;
                holiday.IsActive = true;

                using var conn = _context.CreateConnection();
                conn.Open();

                if (holiday.HolidayId == 0)
                {
                    // new holiday
                    holiday.CreatedById = userId;
                    holiday.CreatedBy = fullName;
                    holiday.CreatedDate = now;

                    var insertSql = @"INSERT INTO Holidays
                                      (
                                        HolidayName,
                                        HolidayDate,
                                        IsRecurring,
                                        Description,
                                        CreatedById,
                                        CreatedBy,
                                        CreatedDate,
                                        ModifiedById,
                                        ModifiedBy,
                                        ModifiedDate
                                      )
                                    VALUES
                                      (
                                        @HolidayName,
                                        @HolidayDate,
                                        @IsRecurring,
                                        @Description,
                                        @CreatedById,
                                        @CreatedBy,
                                        @CreatedDate,
                                        @ModifiedById,
                                        @ModifiedBy,
                                        @ModifiedDate
                                      );";
                    await conn.ExecuteAsync(insertSql, holiday);
                    conn.Close();
                }
                else
                {
                    // update existing
                    var updateSql = @"UPDATE Holidays
                                     SET
                                        HolidayName    = @HolidayName,
                                        HolidayDate    = @HolidayDate,
                                        IsRecurring    = @IsRecurring,
                                        Description    = @Description,
                                        ModifiedById   = @ModifiedById,
                                        ModifiedBy     = @ModifiedBy,
                                        ModifiedDate   = @ModifiedDate
                                     WHERE
                                        HolidayId      = @HolidayId;";
                    await conn.ExecuteAsync(updateSql, holiday);
                    conn.Close();
                }

                return Ok(new { StatusCode = 200 });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500 });
            }
        }

        [HttpGet("GetHolidayById")]
        public async Task<IActionResult> GetHolidayById(int id)
        {
            try
            {
                const string sql = @"SELECT *   FROM Holidays  WHERE HolidayId = @HolidayId;";
                using var conn = _context.CreateConnection();
                conn.Open();
                var holiday = await conn.QueryFirstOrDefaultAsync<Holiday>(sql, new { HolidayId = id });
                conn.Close();
                return Ok(new
                {
                    StatusCode = 200,
                    Holiday = holiday
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500 });
            }
        }

        [HttpPost("DeleteHolidayById")]
        public async Task<IActionResult> DeleteHolidayById(int id)
        {
            try
            {
                const string sql = @"Update Holidays Set IsActive = @IsActive  WHERE HolidayId = @HolidayId;";
                using var conn = _context.CreateConnection();
                conn.Open();
                await conn.ExecuteAsync(sql, new { HolidayId = id, IsActive = 0 });
                conn.Close();
                return Ok(new { StatusCode = 200 });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500 });
            }
        }

        [HttpPost("GetHolidays")]
        public async Task<IActionResult> GetHolidays(HolidayFilter filter)
        {
            try
            {
                var tuple = await FilterHolidaysData(filter);
                return Ok(new
                {
                    StatusCode = 200,
                    Holidays = tuple.Item1,
                    TotalCount = tuple.Item2
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500 });
            }
        }

        // ----------------------
        // Private helper: filtering & paging
        // ----------------------
        private async Task<Tuple<List<Holiday>, int>> FilterHolidaysData(HolidayFilter filter)
        {
            // build WHERE clause
            var where = "WHERE IsActive =1";
            var dynParams = new DynamicParameters();

            if (!string.IsNullOrEmpty(filter.HolidayName))
            {
                where += " AND HolidayName LIKE @Name";
                dynParams.Add("@Name", $"%{filter.HolidayName}%");
            }
            if (filter.DateFrom != null)
            {
                where += " AND HolidayDate >= @DateFrom";
                dynParams.Add("@DateFrom", filter.DateFrom);
            }
            if (filter.DateTo != null)
            {
                where += " AND HolidayDate <= @DateTo";
                dynParams.Add("@DateTo", filter.DateTo);
            }

            // count
            var countSql = $"SELECT COUNT(*) FROM Holidays {where};";

            using var conn = _context.CreateConnection();
            conn.Open();
            var total = await conn.QuerySingleAsync<int>(countSql, filter);

            // fetch page
            var dataSql = $@"SELECT * FROM Holidays {where} ORDER BY HolidayDate DESC OFFSET @iDisplayStart ROWS FETCH NEXT @iDisplayLength ROWS ONLY;";
            var list = (await conn.QueryAsync<Holiday>(dataSql, filter)).AsList();
            return Tuple.Create(list, total);
        }
    }
}

