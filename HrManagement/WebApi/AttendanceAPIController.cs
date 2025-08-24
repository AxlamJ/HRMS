using ClosedXML.Excel;
using Dapper;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Spreadsheet;
using HrManagement.Data;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace HrManagement.WebApi
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceAPIController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AttendanceAPIController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpPost("GetAttendance")]
        public async Task<IActionResult> GetAttendance(AttendanceFilters filters)
        {
            try
            {
                DataTableResponce dtr = new DataTableResponce();
                var tuple = await FilterAttendanceData(filters);
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

        [HttpPost("ExportAttendance")]
        public async Task<IActionResult> ExportAttendance(AttendanceFilters filters)
        {
            try
            {
                filters.iDisplayStart = 0;
                filters.iDisplayLength = 10000;
                DataTableResponce dtr = new DataTableResponce();
                var tuple = await FilterAttendanceData(filters);

                if (tuple.Item2 > 0)
                {
                    // Creating Excel workbook
                    using var workbook = new XLWorkbook();
                    var worksheet = workbook.Worksheets.Add("Attendance Report");
                    var AttendanceEmployeeName = tuple.Item1.Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();

                    int headerRow = 3; // header row index

                    // Adding Header
                    var headers = new string[]
                    {
                        "Employee Code", "First Name", "Last Name", "Date", "Timesheet", "Clock In", "Clock Out",
                        "Clock Time(h)", "Total Break Time(h)", "Total Work Time(h)", "Total Time(h)",
                        "Total Overtime Time(h)", "Statistic Rule Mode", "Abnormal Situation"
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

                    // Running Totals
                    double totalClockTime = 0, totalBreakTime = 0, totalWorkTime = 0, totalTime = 0, totalOvertime = 0;

                    // Inserting Data starting from row 4
                    int startDataRow = headerRow + 1;
                    // Inserting Data
                    for (int i = 0; i < tuple.Item1.Count; i++)
                    {
                        int row = startDataRow + i;
                        var record = tuple.Item1[i];
                        worksheet.Cell(row, 1).Value = record.EmployeeCode;
                        worksheet.Cell(row, 2).Value = record.FirstName;
                        worksheet.Cell(row, 3).Value = record.LastName;
                        worksheet.Cell(row, 4).Value = record.AttendanceDate;
                        worksheet.Cell(row, 5).Value = record.TimeSheet;
                        worksheet.Cell(row, 6).Value = record.ClockIn;
                        worksheet.Cell(row, 7).Value = record.ClockOut;
                        worksheet.Cell(row, 8).Value = record.ClockTime;
                        worksheet.Cell(row, 9).Value = record.TotalBreakTime;
                        worksheet.Cell(row, 10).Value = record.TotalWorkTime;
                        worksheet.Cell(row, 11).Value = record.TotalTime;
                        worksheet.Cell(row, 12).Value = record.TotalOverTime;
                        worksheet.Cell(row, 13).Value = record.StaticRuleMode;
                        worksheet.Cell(row, 14).Value = record.AbnormalSituation;

                        // Applying Borders
                        for (int col = 1; col <= headers.Length; col++)
                        {
                            worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }

                        // Converting Time Strings to Decimal Hours
                        double clockTimeHrs = ConvertToDecimalHours(record.ClockTime);
                        double totalBreakHrs = ConvertToDecimalHours(record.TotalBreakTime);
                        double totalWorkHrs = ConvertToDecimalHours(record.TotalWorkTime);
                        double totalTimeHrs = ConvertToDecimalHours(record.TotalTime);
                        double totalOvertimeHrs = ConvertToDecimalHours(record.TotalOverTime);
                        // Adding to Totals
                        totalClockTime += clockTimeHrs;
                        totalBreakTime += totalBreakHrs;
                        totalWorkTime += totalWorkHrs;
                        totalTime += totalTimeHrs;
                        totalOvertime += totalOvertimeHrs;

                    }

                    var _row = tuple.Item1.Count + 4;
                    // Footer Row (Inserting After Last Row)
                    worksheet.Cell(_row, 7).Value = "TOTAL Time (hrs):";
                    worksheet.Cell(_row, 7).Style.Font.Bold = true;

                    worksheet.Cell(_row, 8).Value = Math.Round(totalClockTime, 2);
                    worksheet.Cell(_row, 9).Value = Math.Round(totalBreakTime, 2);
                    worksheet.Cell(_row, 10).Value = Math.Round(totalWorkTime, 2);
                    worksheet.Cell(_row, 11).Value = Math.Round(totalTime, 2);
                    worksheet.Cell(_row, 12).Value = Math.Round(totalOvertime, 2);

                    // Applying Borders for Footer
                    for (int col = 7; col <= 12; col++)
                    {
                        worksheet.Cell(_row, col).Style.Font.Bold = true;
                        worksheet.Cell(_row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    // Auto adjusting columns
                    worksheet.Columns().AdjustToContents();
                    // Saving to MemoryStream
                    using var stream = new MemoryStream();
                    workbook.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    filters.DateFrom = DateOnly.ParseExact(filters.DateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy");
                    filters.DateTo = DateOnly.ParseExact(filters.DateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy");

                    string FileName = filters.EmployeeCode + "_" + AttendanceEmployeeName + "_" + filters.DateFrom + "-" + filters.DateTo + ".xlsx";

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

        // Function to Convert hh:mm:ss to Decimal Hours
        public static double ConvertToDecimalHours(string timeString)
        {
            if (TimeSpan.TryParseExact(timeString, @"h\:mm\:ss", CultureInfo.InvariantCulture, out TimeSpan time))
            {
                return time.TotalHours; // Convert to decimal hours
            }
            return 0; // Return 0 if parsing fails
        }

        private async Task<Tuple<List<Attendance>, int>> FilterAttendanceData(AttendanceFilters search)
        {
            //var timezoneoffset = Convert.ToInt32(HttpContext.Current.Request.Cookies["timeZoneOffsetCookie"].Value);

            var sqlParams = new List<SqlParameter>();
            var sqlParams1 = new List<SqlParameter>();

            var whereClause = $" Where A.EmployeeCode = @EmployeeCode";

            sqlParams.Add(new SqlParameter("@EmployeeCode", search.EmployeeCode));
            sqlParams1.Add(new SqlParameter("@EmployeeCode", search.EmployeeCode));


            if (!string.IsNullOrEmpty(search.DateFrom))
            {
                search.DateFrom = DateOnly.ParseExact(search.DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");

                whereClause += "\n AND A.AttendanceDate >= @DateFrom";
                sqlParams.Add(new SqlParameter("@DateFrom", search.DateFrom));
                sqlParams1.Add(new SqlParameter("@DateFrom", search.DateFrom));
            }
            if (!string.IsNullOrEmpty(search.DateTo))
            {
                search.DateTo = DateOnly.ParseExact(search.DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");

                whereClause += "\n AND A.AttendanceDate <= @DateTo";
                sqlParams.Add(new SqlParameter("@DateTo", search.DateTo));
                sqlParams1.Add(new SqlParameter("@DateTo", search.DateTo));
            }


            var orderByClause = " ";

            if (string.IsNullOrEmpty(search.SortCol))
            {
                orderByClause = "\n ORDER BY A.AttendanceDate DESC";

            }
            else
            {
                if (search.sSortDir_0.Equals("asc"))
                {
                    if (search.SortCol.Equals("EmployeeCode"))
                    {
                        orderByClause = "\n ORDER BY A.EmployeeCode asc";
                    }
                    else if (search.SortCol.Equals("EmployeeName"))
                    {
                        orderByClause = "\n ORDER BY A.FirstName asc";

                    }
                    else if (search.SortCol.Equals("AttendanceDate"))
                    {
                        orderByClause = "\n ORDER BY A.AttendanceDate asc";
                    }
                }
                if (search.sSortDir_0.Equals("desc"))
                {

                    if (search.SortCol.Equals("EmployeeCode"))
                    {
                        orderByClause = "\n ORDER BY A.EmployeeCode desc";
                    }
                    else if (search.SortCol.Equals("EmployeeName"))
                    {
                        orderByClause = "\n ORDER BY A.FirstName desc";

                    }
                    else if (search.SortCol.Equals("AttendanceDate"))
                    {
                        orderByClause = "\n ORDER BY A.AttendanceDate desc";
                    }
                }

            }

            var joins = @"";

            var sqlForCount = @"SELECT COUNT(A.AttendanceId) FROM dbo.EmployeeAttendance A "
               + joins
               + whereClause;


            using var connection = _context.CreateConnection();
            connection.Open();
            var totalCount = await connection.QuerySingleAsync<int>(sqlForCount, search);
            //connection.Close();

            var sql = @"SELECT 
	                         A.AttendanceID
                            ,A.EmployeeCode
                            ,A.FirstName
                            ,A.LastName
                            ,A.AttendanceDate
                            ,A.Timesheet
                            ,A.ClockIn
                            ,A.ClockOut
                            ,A.ClockTime
                            ,A.TotalBreakTime
                            ,A.TotalWorkTime
                            ,A.TotalTime
                            ,A.TotalOvertime
                            ,A.StaticRuleMode
                            ,A.AbnormalSituation
                            FROM dbo.EmployeeAttendance A"
                        + joins
                        + whereClause
                        + orderByClause
                        + "\n OFFSET @iDisplayStart ROWS FETCH NEXT @iDisplayLength ROWS ONLY;";
            var data = await connection.QueryAsync<Attendance>(sql, search);
            var EmployeeAttendanceList = data.ToList<Attendance>();
            //var data = db.Database.SqlQuery<Sites>(sql, sqlParams.ToArray()).ToList();
            return new Tuple<List<Attendance>, int>(EmployeeAttendanceList, totalCount);

        }
    }
}
