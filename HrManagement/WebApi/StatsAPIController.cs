using Dapper;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using HrManagement.Data;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace HrManagement.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsAPIController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StatsAPIController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("GetDashBoardStats")]
        public async Task<ActionResult> DashBoardStats()
        {
            DashboardStats dashboardStats = new DashboardStats();
            try
            {
                var GenderStatsQuery = @"SELECT 
                                            CASE 
                                                WHEN Gender IS NULL OR LTRIM(RTRIM(Gender)) = '' THEN 'Gender-Neutral'
                                                ELSE Gender
                                            END AS name,
                                            CAST(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER() AS DECIMAL(5,2)) AS value
                                        FROM 
                                            Employee
                                        GROUP BY 
                                            CASE 
                                                WHEN Gender IS NULL OR LTRIM(RTRIM(Gender)) = '' THEN 'Gender-Neutral'
                                                ELSE Gender
                                            END";

                using var connection = _context.CreateConnection();
                connection.Open();
                var genderstatsdata = await connection.QueryAsync<StatsVm>(GenderStatsQuery);
                var genderstats = genderstatsdata.ToList<StatsVm>();

                dashboardStats.GenderStats.AddRange(genderstats);

                var AgeStatsQuery = @"SELECT 
                                        CONCAT(
                                            ((DATEDIFF(YEAR, DOB, GETDATE()) - 1) / 10) * 10 + 1,
                                            '-',
                                            ((DATEDIFF(YEAR, DOB, GETDATE()) - 1) / 10 + 1) * 10
                                        ) AS name,
                                        CAST(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER() AS DECIMAL(5,2)) AS value
                                    FROM Employee
                                    WHERE DOB IS NOT NULL
                                    GROUP BY 
                                        ((DATEDIFF(YEAR, DOB, GETDATE()) - 1) / 10)
                                    ORDER BY 
                                        ((DATEDIFF(YEAR, DOB, GETDATE()) - 1) / 10)";

                var agestatsdata = await connection.QueryAsync<StatsVm>(AgeStatsQuery);
                var agestats = agestatsdata.ToList<StatsVm>();

                dashboardStats.AgeStats.AddRange(agestats);


                var SiteStatsQuery = @"SELECT 
                                            ISNULL(NULLIF(LTRIM(RTRIM(Name)), ''), 'Unspecified') AS name,
                                            CAST(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER() AS DECIMAL(5,2)) AS value
                                        FROM (
                                            SELECT 
                                                JSONData.name AS Name
                                            FROM Employee
                                            CROSS APPLY OPENJSON(SiteName)
                                            WITH (
                                                id INT,
                                                name NVARCHAR(200)
                                            ) AS JSONData
                                        ) AS ParsedNames
                                        GROUP BY 
                                            ISNULL(NULLIF(LTRIM(RTRIM(Name)), ''), 'Unspecified');";

                var sitestatsdata = await connection.QueryAsync<StatsVm>(SiteStatsQuery);
                var sitestats = sitestatsdata.ToList<StatsVm>();

                dashboardStats.SiteStats.AddRange(sitestats);


                var DepartmentStatsQuery = @"SELECT 
                                                ISNULL(NULLIF(LTRIM(RTRIM(Name)), ''), 'Unspecified') AS name,
                                                CAST(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER() AS DECIMAL(5,2)) AS value
                                            FROM (
                                                SELECT 
                                                    JSONData.name AS Name
                                                FROM Employee
                                                CROSS APPLY OPENJSON(SiteName)
                                                WITH (
                                                    id INT,
                                                    name NVARCHAR(200)
                                                ) AS JSONData
                                            ) AS ParsedNames
                                            GROUP BY 
                                                ISNULL(NULLIF(LTRIM(RTRIM(Name)), ''), 'Unspecified')";

                var departmentstatsdata = await connection.QueryAsync<StatsVm>(DepartmentStatsQuery);
                var departmentstats = departmentstatsdata.ToList<StatsVm>();

                dashboardStats.DepartmentStats.AddRange(departmentstats);

                var HeadCountStatsQuery = @"WITH Months AS (
                                                SELECT TOP 12 
                                                    DATEFROMPARTS(YEAR(DATEADD(MONTH, -number, GETDATE())), MONTH(DATEADD(MONTH, -number, GETDATE())), 1) AS MonthStart
                                                FROM master.dbo.spt_values
                                                WHERE type = 'P' AND number BETWEEN 1 AND 12
                                                ORDER BY MonthStart
                                            )
                                            SELECT 
                                                FORMAT(m.MonthStart, 'MMM, yyyy') AS name,
                                                COUNT(e.Id) AS value
                                            FROM Months m
                                            LEFT JOIN Employee e
                                                ON e.HiringDate <= EOMONTH(m.MonthStart)
                                            GROUP BY m.MonthStart
                                            ORDER BY m.MonthStart
                                            ";

                var headcountstatsdata = await connection.QueryAsync<StatsVm>(HeadCountStatsQuery);
                var headcountstats = headcountstatsdata.ToList<StatsVm>();

                var lineChart = new LineCharts();

                lineChart.xAxis.AddRange(headcountstats.Select(x => x.name).ToArray());
                lineChart.yAxisSeries.AddRange(headcountstats.Select(x => x.value).ToArray());

                dashboardStats.HeadCountStats = lineChart;


                var HiredvsLeftStatsQuery = @"WITH Months AS (
                                                SELECT TOP 12 
                                                    DATEFROMPARTS(YEAR(DATEADD(MONTH, -number, GETDATE())), MONTH(DATEADD(MONTH, -number, GETDATE())), 1) AS MonthStart,
                                                    EOMONTH(DATEADD(MONTH, -number, GETDATE())) AS MonthEnd
                                                FROM master.dbo.spt_values
                                                WHERE type = 'P' AND number BETWEEN 1 AND 12 -- Exclude current month
                                                ORDER BY MonthStart
                                            )
                                            SELECT 
                                                FORMAT(m.MonthStart, 'MMM, yyyy') AS MonthYear,
                                                COUNT(h.Id) AS Hired,
                                                COUNT(t.Id) AS 'Left'
                                            FROM Months m
                                            LEFT JOIN Employee h
                                                ON h.HiringDate >= m.MonthStart AND h.HiringDate <= m.MonthEnd
                                            LEFT JOIN Employee t
                                                ON t.TerminationDismissalDate IS NOT NULL AND t.TerminationDismissalDate >= m.MonthStart AND t.TerminationDismissalDate <= m.MonthEnd
                                            GROUP BY m.MonthStart
                                            ORDER BY m.MonthStart;
                                            ";

                var hiredvsleftstatsdata = await connection.QueryAsync<MonthlyHiredLeft>(HiredvsLeftStatsQuery);
                var hiredvsleftstats = hiredvsleftstatsdata.ToList<MonthlyHiredLeft>();

                var BarChart = new LineCharts();

                dashboardStats.HiredVsLeft.AddRange(hiredvsleftstats);

                var UpComingOccassionsQuery = @"DECLARE @Today DATE = GETDATE();
                                                DECLARE @EndDate DATE = DATEADD(MONTH, 12, @Today);
                                                DECLARE @CurrentYear INT = YEAR(@Today);

                                                WITH AnniversaryEvents AS (
                                                    SELECT 
                                                        CONCAT(FirstName, ' ',LastName) as EmployeeName,
                                                        DATEFROMPARTS(@CurrentYear, MONTH(DOB), DAY(DOB)) AS EventDate,
                                                        'Birthday' AS EventType
                                                    FROM Employee
                                                    WHERE DATEFROMPARTS(@CurrentYear, MONTH(DOB), DAY(DOB)) 
                                                          BETWEEN @Today AND @EndDate  and  Status in (1,2)

                                                    UNION ALL

                                                    SELECT 
                                                        CONCAT(FirstName, ' ',LastName) as EmployeeName,  
		                                                DATEFROMPARTS(@CurrentYear, MONTH(HiringDate), DAY(HiringDate)) AS EventDate,
                                                        CAST(Years AS VARCHAR(10)) + 
                                                        CASE 
                                                            WHEN Years % 100 IN (11, 12, 13) THEN 'th'
                                                            WHEN Years % 10 = 1 THEN 'st'
                                                            WHEN Years % 10 = 2 THEN 'nd'
                                                            WHEN Years % 10 = 3 THEN 'rd'
                                                            ELSE 'th'
                                                        END + ' Work Anniversary' AS EventType
                                                    FROM (
                                                        SELECT *,
                                                               DATEDIFF(YEAR, HiringDate, DATEFROMPARTS(@CurrentYear, MONTH(HiringDate), DAY(HiringDate))) AS Years
                                                        FROM Employee  where Status in (1,2)
                                                    ) AS Sub
                                                    WHERE 
                                                        Years > 0 AND
                                                        DATEFROMPARTS(@CurrentYear, MONTH(HiringDate), DAY(HiringDate)) 
                                                        BETWEEN @Today AND @EndDate
                                                )
                                                SELECT *
                                                FROM AnniversaryEvents
                                                ORDER BY EventDate;";

                var upcomingoccasions = await connection.QueryAsync<UpCommingoccasion>(UpComingOccassionsQuery);
                var upcomingoccasionslist = upcomingoccasions.ToList<UpCommingoccasion>();

                var EmployeeCountQuery = @"DECLARE @Today DATE = GETDATE();
                                                DECLARE @EndDate DATE = DATEADD(MONTH, 12, @Today);
                                                DECLARE @CurrentYear INT = YEAR(@Today);

                                                WITH AnniversaryEvents AS (
                                                    SELECT 
                                                        CONCAT(FirstName, ' ',LastName) as EmployeeName,
                                                        DATEFROMPARTS(@CurrentYear, MONTH(DOB), DAY(DOB)) AS EventDate,
                                                        'Birthday' AS EventType
                                                    FROM Employee
                                                    WHERE DATEFROMPARTS(@CurrentYear, MONTH(DOB), DAY(DOB)) 
                                                          BETWEEN @Today AND @EndDate  and  Status in (1,2)

                                                    UNION ALL

                                                    SELECT 
                                                        CONCAT(FirstName, ' ',LastName) as EmployeeName,  
		                                                DATEFROMPARTS(@CurrentYear, MONTH(HiringDate), DAY(HiringDate)) AS EventDate,
                                                        CAST(Years AS VARCHAR(10)) + 
                                                        CASE 
                                                            WHEN Years % 100 IN (11, 12, 13) THEN 'th'
                                                            WHEN Years % 10 = 1 THEN 'st'
                                                            WHEN Years % 10 = 2 THEN 'nd'
                                                            WHEN Years % 10 = 3 THEN 'rd'
                                                            ELSE 'th'
                                                        END + ' Work Anniversary' AS EventType
                                                    FROM (
                                                        SELECT *,
                                                               DATEDIFF(YEAR, HiringDate, DATEFROMPARTS(@CurrentYear, MONTH(HiringDate), DAY(HiringDate))) AS Years
                                                        FROM Employee  where Status in (1,2)
                                                    ) AS Sub
                                                    WHERE 
                                                        Years > 0 AND
                                                        DATEFROMPARTS(@CurrentYear, MONTH(HiringDate), DAY(HiringDate)) 
                                                        BETWEEN @Today AND @EndDate
                                                )
                                                SELECT *
                                                FROM AnniversaryEvents
                                                ORDER BY EventDate;";

                var employeecount = await connection.QueryAsync<EmployeeCount>(EmployeeCountQuery);
                var employeesCount = employeecount.FirstOrDefault();

                dashboardStats.EmployeesCount = employeesCount;



                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    DashBoardStats = dashboardStats
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    DashBoardStats = dashboardStats
                });
            }
        }

        [HttpGet("GetGenderStats")]
        public async Task<ActionResult> GetGenderStats()
        {
            List<StatsVm> stats = new List<StatsVm>();
            try
            {

                using var connection = _context.CreateConnection();
                connection.Open();

                var GenderStatsQuery = @"SELECT 
                                            CASE 
                                                WHEN Gender IS NULL OR LTRIM(RTRIM(Gender)) = '' THEN 'Gender-Neutral'
                                                ELSE Gender
                                            END AS name,
                                            CAST(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER() AS DECIMAL(5,2)) AS value,
                                            COUNT(*) as totalcount
                                        FROM 
                                            Employee
                                        GROUP BY 
                                            CASE 
                                                WHEN Gender IS NULL OR LTRIM(RTRIM(Gender)) = '' THEN 'Gender-Neutral'
                                                ELSE Gender
                                            END";

                var genderstatsdata = await connection.QueryAsync<StatsVm>(GenderStatsQuery);
                var genderstats = genderstatsdata.ToList<StatsVm>();

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    GenderStats = genderstats
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    GenderStats = stats
                });
            }
        }

        [HttpGet("GetSiteStats")]
        public async Task<ActionResult> GetSiteStats()
        {
            List<StatsVm> stats = new List<StatsVm>();
            try
            {

                using var connection = _context.CreateConnection();
                connection.Open();

                var SiteStatsQuery = @"SELECT 
                                            ISNULL(NULLIF(LTRIM(RTRIM(Name)), ''), 'Unspecified') AS name,
                                            CAST(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER() AS DECIMAL(5,2)) AS value,
                                            COUNT(*) AS totalcount
                                        FROM (
                                            SELECT 
                                                j.[name] AS Name
                                            FROM Employee e
                                            CROSS APPLY OPENJSON(e.SiteName)
                                            WITH (
                                                id INT,
                                                name NVARCHAR(200)
                                            ) AS j
                                        ) AS parsed
                                        GROUP BY 
                                            ISNULL(NULLIF(LTRIM(RTRIM(Name)), ''), 'Unspecified');";

                var sitestatsdata = await connection.QueryAsync<StatsVm>(SiteStatsQuery);
                var sitestats = sitestatsdata.ToList<StatsVm>();

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    SiteStats = sitestats
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    SiteStats = stats
                });
            }
        }

        [HttpGet("GetAgeStats")]
        public async Task<ActionResult> GetAgeStats()
        {
            List<StatsVm> stats = new List<StatsVm>();
            try
            {

                using var connection = _context.CreateConnection();
                connection.Open();

                var StatsQuery = @"SELECT 
                                        CONCAT(
                                            ((DATEDIFF(YEAR, DOB, GETDATE()) - 1) / 10) * 10 + 1,
                                            '-',
                                            ((DATEDIFF(YEAR, DOB, GETDATE()) - 1) / 10 + 1) * 10
                                        ) AS name,
                                        CAST(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER() AS DECIMAL(5,2)) AS value,
                                        COUNT(*) as totalcount
                                    FROM Employee
                                    WHERE DOB IS NOT NULL
                                    GROUP BY 
                                        ((DATEDIFF(YEAR, DOB, GETDATE()) - 1) / 10)
                                    ORDER BY 
                                        ((DATEDIFF(YEAR, DOB, GETDATE()) - 1) / 10)";

                var statsdata = await connection.QueryAsync<StatsVm>(StatsQuery);
                var agestats = statsdata.ToList<StatsVm>();

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    AgeStats = agestats
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    AgeStats = stats
                });
            }
        }


        [HttpGet("GetDepartmentStats")]
        public async Task<ActionResult> GetDepartmentStats()
        {
            List<StatsVm> stats = new List<StatsVm>();
            try
            {

                using var connection = _context.CreateConnection();
                connection.Open();

                var StatsQuery = @"SELECT 
                                        ISNULL(NULLIF(LTRIM(RTRIM(Name)), ''), 'Unspecified') AS name,
                                        CAST(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER() AS DECIMAL(5,2)) AS value,
                                        COUNT(*) AS totalcount
                                    FROM (
                                        SELECT 
                                            j.[name] AS Name
                                        FROM Employee e
                                        CROSS APPLY OPENJSON(e.DepartmentName)
                                        WITH (
                                            id INT,
                                            name NVARCHAR(200)
                                        ) AS j
                                    ) AS parsed
                                    GROUP BY 
                                        ISNULL(NULLIF(LTRIM(RTRIM(Name)), ''), 'Unspecified');
                                    ";

                var statsdata = await connection.QueryAsync<StatsVm>(StatsQuery);
                var departmenttats = statsdata.ToList<StatsVm>();

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    DepartmentStats = departmenttats
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    DepartmentStats = stats
                });
            }
        }


        [HttpGet("GetHeadCountStats")]
        public async Task<ActionResult> GetHeadCountStats(string? StartMonth, string? StartYear, string? EndMonth, string? EndYear)
        {
            LineCharts stats = new LineCharts();
            try
            {

                using var connection = _context.CreateConnection();
                connection.Open();

                var StatsQuery = @"DECLARE @StartDate DATE = DATEFROMPARTS(@StartYear, @StartMonth, 1);
                                   DECLARE @EndDate DATE = EOMONTH(DATEFROMPARTS(@EndYear, @EndMonth, 1));

                                   WITH Months AS (
                                       SELECT DATEFROMPARTS(YEAR(@StartDate), MONTH(@StartDate), 1) AS MonthStart
                                       UNION ALL
                                       SELECT DATEADD(MONTH, 1, MonthStart)
                                       FROM Months
                                       WHERE DATEADD(MONTH, 1, MonthStart) <= @EndDate
                                   )
                                   SELECT 
                                       FORMAT(m.MonthStart, 'MMM, yyyy') AS name,
                                       COUNT(e.Id) AS value
                                   FROM Months m
                                   LEFT JOIN Employee e
                                       ON e.HiringDate <= EOMONTH(m.MonthStart)
                                   GROUP BY m.MonthStart
                                   ORDER BY m.MonthStart
                                   OPTION (MAXRECURSION 0);
                                   ";

                var paramas = new
                {
                    StartMonth = StartMonth,
                    StartYear = StartYear,
                    EndMonth = EndMonth,
                    EndYear = EndYear
                };

                var statsdata = await connection.QueryAsync<StatsVm>(StatsQuery,paramas);
                var headcountstats = statsdata.ToList<StatsVm>();


                var lineChart = new LineCharts();

                lineChart.xAxis.AddRange(headcountstats.Select(x => x.name).ToArray());
                lineChart.yAxisSeries.AddRange(headcountstats.Select(x => x.value).ToArray());

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    HeadCountStats = lineChart
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    HeadCountStats = stats
                });
            }
        }

        [HttpGet("GetHiredVsLeftStats")]
        public async Task<ActionResult> GetHiredVsLeftStats(string? StartMonth, string? StartYear, string? EndMonth, string? EndYear)
        {
            List<MonthlyHiredLeft> stats = new List<MonthlyHiredLeft>();
            try
            {

                using var connection = _context.CreateConnection();
                connection.Open();

                var StatsQuery = @"DECLARE @StartDate DATE = DATEFROMPARTS(@StartYear, @StartMonth, 1);
                                   DECLARE @EndDate DATE = EOMONTH(DATEFROMPARTS(@EndYear, @EndMonth, 1));

                                   WITH Months AS (
                                        SELECT DATEFROMPARTS(YEAR(@StartDate), MONTH(@StartDate), 1) AS MonthStart,
                                               EOMONTH(DATEFROMPARTS(YEAR(@StartDate), MONTH(@StartDate), 1)) AS MonthEnd
                                        UNION ALL
                                        SELECT 
                                            DATEADD(MONTH, 1, MonthStart),
                                            EOMONTH(DATEADD(MONTH, 1, MonthStart))
                                        FROM Months
                                        WHERE DATEADD(MONTH, 1, MonthStart) <= EOMONTH(@EndDate)
                                    )
                                    SELECT 
                                        FORMAT(m.MonthStart, 'MMM, yyyy') AS MonthYear,
                                        COUNT(h.Id) AS Hired,
                                        COUNT(t.Id) AS [Left]
                                    FROM Months m
                                    LEFT JOIN Employee h
                                        ON h.HiringDate >= m.MonthStart AND h.HiringDate <= m.MonthEnd
                                    LEFT JOIN Employee t
                                        ON t.TerminationDismissalDate IS NOT NULL 
                                           AND t.TerminationDismissalDate >= m.MonthStart 
                                           AND t.TerminationDismissalDate <= m.MonthEnd
                                    GROUP BY m.MonthStart
                                    ORDER BY m.MonthStart
                                    OPTION (MAXRECURSION 0);
                                   ";

                var paramas = new
                {
                    StartMonth = StartMonth,
                    StartYear = StartYear,
                    EndMonth = EndMonth,
                    EndYear = EndYear
                };


                var hiredvsleftstatsdata = await connection.QueryAsync<MonthlyHiredLeft>(StatsQuery,paramas);
                var hiredvsleftstats = hiredvsleftstatsdata.ToList<MonthlyHiredLeft>();

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    HiredVsLeftStats = hiredvsleftstats
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    HiredVsLeftStats = stats
                });
            }
        }


        [HttpGet("GetEmployeeLevelStats")]
        public async Task<ActionResult> GetEmployeeLevelStats()
        {
            List<StatsVm> stats = new List<StatsVm>();
            try
            {

                using var connection = _context.CreateConnection();
                connection.Open();

                var LevelStatsQuery = @"WITH LevelCounts AS (
                                          SELECT 
                                              ISNULL(NULLIF(LTRIM(RTRIM(EmploymentLevel)), ''), 'Unspecified') AS Name,
                                              COUNT(*) AS TotalCount
                                          FROM Employee
                                          GROUP BY ISNULL(NULLIF(LTRIM(RTRIM(EmploymentLevel)), ''), 'Unspecified')
                                      ),
                                      GrandTotal AS (
                                          SELECT SUM(TotalCount) AS AllEmployees FROM LevelCounts
                                      )
                                      SELECT 
                                          lc.Name,
                                          lc.TotalCount,
                                          CAST(ROUND((1.0 * lc.TotalCount / gt.AllEmployees) * 100, 2) AS DECIMAL(5,2)) AS Value
                                      FROM LevelCounts lc
                                      CROSS JOIN GrandTotal gt
                                      ORDER BY lc.TotalCount DESC;";

                var levelstatsdata = await connection.QueryAsync<StatsVm>(LevelStatsQuery);
                var levelstats = levelstatsdata.ToList<StatsVm>();

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    EmployeeLevelStats = levelstats
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    SiteStats = stats
                });
            }
        }

        [HttpGet("GetTerminationDismissalReasonStats")]
        public async Task<ActionResult> GetTerminationDismissalReasonStats()
        {
            List<StatsVm> stats = new List<StatsVm>();
            try
            {

                using var connection = _context.CreateConnection();
                connection.Open();

                var TerminationStatsQuery = @"WITH TerminationCounts AS (
                                                 SELECT 
                                                     ISNULL(NULLIF(LTRIM(RTRIM(TR.ReasonName)), ''), 'Unspecified') AS Name,
                                                     COUNT(E.Id) AS TotalCount
                                                 FROM TerminationDismissalReason TR
                                                 LEFT JOIN Employee E ON E.TerminationDismissalReasonId = TR.Id AND E.Status = 3
                                                 GROUP BY ISNULL(NULLIF(LTRIM(RTRIM(TR.ReasonName)), ''), 'Unspecified')
                                             ),
                                             GrandTotal AS (
                                                 SELECT SUM(TotalCount) AS AllTerminated FROM TerminationCounts
                                             )
                                             SELECT 
                                                 tc.Name,
                                                 tc.TotalCount,
                                                 CAST(ROUND(
                                                     CASE 
                                                         WHEN gt.AllTerminated = 0 THEN 0 
                                                         ELSE (1.0 * tc.TotalCount / gt.AllTerminated) * 100 
                                                     END, 2
                                                 ) AS DECIMAL(5,2)) AS Value
                                             FROM TerminationCounts tc
                                             CROSS JOIN GrandTotal gt
                                             ORDER BY tc.TotalCount DESC;";

                var terminationstatsdata = await connection.QueryAsync<StatsVm>(TerminationStatsQuery);
                var terminationstats = terminationstatsdata.ToList<StatsVm>();

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    TerminationStats = terminationstats
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    SiteStats = stats
                });
            }
        }

        [HttpGet("GetTerminationDismissalTypeStats")]
        public async Task<ActionResult> GetTerminationDismissalTypeStats()
        {
            List<StatsVm> stats = new List<StatsVm>();
            try
            {

                using var connection = _context.CreateConnection();
                connection.Open();

                var TerminationTypeStatsQuery = @"WITH TerminationCounts AS (
                                                     SELECT 
                                                         ISNULL(NULLIF(LTRIM(RTRIM(TerminationDismissalType)), ''), 'Unspecified') AS Name,
                                                         COUNT(*) AS TotalCount
                                                     FROM Employee
                                                     WHERE Status = 3
                                                     GROUP BY ISNULL(NULLIF(LTRIM(RTRIM(TerminationDismissalType)), ''), 'Unspecified')
                                                 ),
                                                 GrandTotal AS (
                                                     SELECT SUM(TotalCount) AS AllTerminated FROM TerminationCounts
                                                 )
                                                 SELECT 
                                                     tc.Name,
                                                     tc.TotalCount,
                                                     CAST(ROUND(
                                                         CASE 
                                                             WHEN gt.AllTerminated = 0 THEN 0 
                                                             ELSE (1.0 * tc.TotalCount / gt.AllTerminated) * 100 
                                                         END, 2
                                                     ) AS DECIMAL(5,2)) AS Value
                                                 FROM TerminationCounts tc
                                                 CROSS JOIN GrandTotal gt
                                                 ORDER BY tc.TotalCount DESC;";

                var terminationtypestatsdata = await connection.QueryAsync<StatsVm>(TerminationTypeStatsQuery);
                var terminationtypestats = terminationtypestatsdata.ToList<StatsVm>();

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    TerminationTypeStats = terminationtypestats
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    SiteStats = stats
                });
            }
        }

        [HttpGet("GetBirthDayList")]
        public async Task<ActionResult> GetBirthDayList(string? FirstName,string? LastName,int? Month)
        {
            List<StatsVm> stats = new List<StatsVm>();
            try
            {

                using var connection = _context.CreateConnection();
                connection.Open();

                if (Month == 0) Month = null;
                var TerminationTypeStatsQuery = @"DECLARE @Today DATE = GETDATE();
                                                  DECLARE @EndOfYear DATE = EOMONTH(DATEFROMPARTS(YEAR(@Today), 12, 1));
                                                  DECLARE @CurrentYear INT = YEAR(@Today);
                                                  
                                                  
                                                  SELECT 
                                                      CONCAT(FirstName,' ',    LastName) AS EmployeeName,
                                                      DATEFROMPARTS(@CurrentYear, MONTH(DOB), DAY(DOB)) AS EventDate,
                                                      'Birthday' AS EventType
                                                  FROM Employee
                                                  WHERE 
                                                      Status IN (1, 2)
                                                      AND (
                                                          -- Default: from today to end of year
                                                          (@FilterMonth IS NULL AND DATEFROMPARTS(@CurrentYear, MONTH(DOB), DAY(DOB)) BETWEEN @Today AND @EndOfYear)
                                                          -- If filter month is provided
                                                          OR
                                                          (@FilterMonth IS NOT NULL AND MONTH(DOB) = @FilterMonth)
                                                      )
                                                      AND (
                                                          @FirstName IS NULL OR FirstName LIKE '%' + @FirstName + '%'
                                                      )
                                                      AND(
	                                                  @LastName IS NULL OR LastName LIKE '%' + @LastName + '%'
                                                      )
                                                  ORDER BY EventDate;";

                var _params = new { FirstName = FirstName, LastName = LastName, FilterMonth = Month};
                var birthdaylistdata = await connection.QueryAsync<UpCommingoccasion>(TerminationTypeStatsQuery,_params);
                var birthdaylist = birthdaylistdata.ToList<UpCommingoccasion>();

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    BirthDayList = birthdaylist
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    SiteStats = stats
                });
            }
        }


        [HttpGet("GetWorkDurationStats")]
        public async Task<ActionResult> GetWorkDurationStats()
        {
            List<StatsVm> stats = new List<StatsVm>();
            try
            {

                using var connection = _context.CreateConnection();
                connection.Open();

                var WorkDurationStatsQuery = @"WITH ProbationLengths AS (
                                            SELECT
                                                DATEDIFF(YEAR, HiringDate, GETDATE()) AS YearsOnProbation,
                                                COUNT(*) AS TotalCount
                                            FROM
                                                Employee
                                            WHERE
                                                HiringDate IS NOT NULL
                                            GROUP BY
                                                DATEDIFF(YEAR, HiringDate, GETDATE())
                                        ),
                                        GrandTotal AS (
                                            SELECT SUM(TotalCount) AS AllEmployees
                                            FROM ProbationLengths
                                        )
                                        SELECT
                                            CAST(pl.YearsOnProbation AS varchar(3))
                                              + ' Year'
                                              + CASE WHEN pl.YearsOnProbation = 1 THEN '' ELSE 's' END
                                              AS Name,
                                            pl.TotalCount,
                                            CAST(
                                              ROUND(
                                                (1.0 * pl.TotalCount / gt.AllEmployees) * 100
                                              , 2)
                                            AS DECIMAL(5,2)) AS Value
                                        FROM
                                            ProbationLengths pl
                                            CROSS JOIN GrandTotal gt
                                        ORDER BY
                                            pl.YearsOnProbation;";

                var workdurationdata = await connection.QueryAsync<StatsVm>(WorkDurationStatsQuery);
                var workdurationstats = workdurationdata.ToList<StatsVm>();

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    WorkDurationStats = workdurationstats
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    SiteStats = stats
                });
            }
        }

    }
}
