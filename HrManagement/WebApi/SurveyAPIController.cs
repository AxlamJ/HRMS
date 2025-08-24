using Dapper;
using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;
using DocumentFormat.OpenXml.Office2013.Excel;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
using HrManagement.Data;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using static Azure.Core.HttpHeader;

namespace HrManagement.WebApi
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyAPIController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SurveyAPIController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("Save")]  // Save Survey Data
        public async Task<IActionResult> SaveSurvey([FromBody] SurveyDto survey)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                if (survey != null)
                {
                    using (var connection = _context.CreateConnection())
                    {
                        connection.Open();

                        #region Saving and updating Survey.

                        if (survey.Id == 0)
                        {
                            var insertSurvey = @"INSERT INTO Surveys 
                                (
                                     Name
                                    ,Description
                                    ,Status
                                    ,IsRecurring
                                    ,Recursion
                                    ,PublishDate
                                    ,CompletionDate
                                    ,SiteId
                                    ,Site
                                    ,DepartmentId
                                    ,Department
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
                                     @Name
                                    ,@Description
                                    ,@Status
                                    ,@IsRecurring
                                    ,@Recursion
                                    ,@PublishDate
                                    ,@CompletionDate
                                    ,@SiteId
                                    ,@Site
                                    ,@DepartmentId
                                    ,@Department
                                    ,@CreatedById
                                    ,@CreatedBy
                                    ,@CreatedDate
                                    ,@ModifiedById
                                    ,@ModifiedBy
                                    ,@ModifiedDate
                                    ,@IsActive
                                );
                                SELECT CAST(SCOPE_IDENTITY() as int);";

                            if (!string.IsNullOrEmpty(survey.PublishDate))
                            {
                                survey.PublishDate = DateTime.ParseExact(survey.PublishDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            }
                            if (!string.IsNullOrEmpty(survey.CompletionDate))
                            {
                                survey.CompletionDate = DateTime.ParseExact(survey.CompletionDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            }

                            survey.CreatedById = loggedinUserId;
                            survey.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                            survey.CreatedDate = DateTime.UtcNow;
                            survey.ModifiedById = loggedinUserId;
                            survey.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                            survey.ModifiedDate = DateTime.UtcNow;
                            survey.IsActive = true;

                            int surveyId = connection.QuerySingle<int>(insertSurvey, survey);
                            survey.Id = surveyId;
                        }
                        else
                        {
                            var updateSurvey = @"UPDATE Surveys
                                SET 
                                     Name = @Name
                                    ,Description = @Description
                                    ,Status = @Status
                                    ,IsRecurring = @IsRecurring
                                    ,Recursion = @Recursion
                                    ,PublishDate = @PublishDate
                                    ,CompletionDate = @CompletionDate
                                    ,SiteId = @SiteId
                                    ,Site = @Site
                                    ,DepartmentId = @DepartmentId
                                    ,Department = @Department
                                    ,ModifiedById = @ModifiedById
                                    ,ModifiedBy = @ModifiedBy
                                    ,ModifiedDate = @ModifiedDate
                                WHERE Id = @Id";

                            if (!string.IsNullOrEmpty(survey.PublishDate))
                            {
                                survey.PublishDate = DateTime.ParseExact(survey.PublishDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            }
                            if (!string.IsNullOrEmpty(survey.CompletionDate))
                            {
                                survey.CompletionDate = DateTime.ParseExact(survey.CompletionDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            }

                            survey.ModifiedById = loggedinUserId;
                            survey.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                            survey.ModifiedDate = DateTime.UtcNow;

                            await connection.ExecuteAsync(updateSurvey, survey);
                        }

                        #endregion

                        #region Saving and updating survey employees.

                        string deleteEmployees = "DELETE FROM SurveyEmployees WHERE SurveyId = @SurveyId;";
                        await connection.ExecuteAsync(deleteEmployees, new { SurveyId = survey.Id });

                        foreach (var employee in survey.Employees)
                        {
                            string insertOption = @"INSERT INTO SurveyEmployees 
                            (
                                 SurveyId
                                ,EmployeeCode
                                ,EmployeeName
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
                                 @SurveyId
                                ,@EmployeeCode
                                ,@EmployeeName
                                ,@CreatedById
                                ,@CreatedBy
                                ,@CreatedDate
                                ,@ModifiedById
                                ,@ModifiedBy
                                ,@ModifiedDate
                                ,@IsActive
                            );";

                            connection.Execute(insertOption, new
                            {
                                SurveyId = survey.Id,
                                employee.EmployeeCode,
                                employee.EmployeeName,
                                CreatedById = loggedinUserId,
                                CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedById = loggedinUserId,
                                ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName,
                                ModifiedDate = DateTime.UtcNow,
                                IsActive = true
                            });

                        }

                        #endregion

                        #region Saving and updating Questions and Options.

                        string deleteQuestions = "DELETE FROM SurveyQuestions WHERE SurveyId = @SurveyId;";
                        await connection.ExecuteAsync(deleteQuestions, new { SurveyId = survey.Id });

                        foreach (var question in survey.Questions)
                        {
                            string insertQuestion = @"INSERT INTO SurveyQuestions 
                                    (
                                         SurveyId
                                        ,QuestionText
                                        ,Description
                                        ,QuestionType
                                        ,IsRequired
                                        ,SortOrder
                                        ,MinValue
                                        ,MaxValue
                                        ,Scale
                                        ,Shape
                                        ,Label
                                        ,Weight
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
                                         @SurveyId
                                        ,@QuestionText
                                        ,@Description
                                        ,@QuestionType
                                        ,@IsRequired
                                        ,@SortOrder
                                        ,@MinValue
                                        ,@MaxValue
                                        ,@Scale
                                        ,@Shape
                                        ,@Label
                                        ,@Weight
                                        ,@CreatedById
                                        ,@CreatedBy
                                        ,@CreatedDate
                                        ,@ModifiedById
                                        ,@ModifiedBy
                                        ,@ModifiedDate
                                        ,@IsActive
                                    );
                                    SELECT CAST(SCOPE_IDENTITY() as int);";

                            int questionId = connection.QuerySingle<int>(insertQuestion, new
                            {
                                SurveyId = survey.Id,
                                question.QuestionText,
                                question.Description,
                                question.QuestionType,
                                question.IsRequired,
                                question.SortOrder,
                                question.MinValue,
                                question.MaxValue,
                                question.Scale,
                                question.Shape,
                                question.Label,
                                question.Weight,
                                CreatedById = loggedinUserId,
                                CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedById = loggedinUserId,
                                ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName,
                                ModifiedDate = DateTime.UtcNow,
                                IsActive = true
                            });

                            foreach (var option in question.Options)
                            {
                                string insertOption = @"INSERT INTO SurveyQuestionOptions 
                                        (
                                             QuestionId
                                            ,OptionText
                                            ,SortOrder
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
                                             @QuestionId
                                            ,@OptionText
                                            ,@SortOrder
                                            ,@CreatedById
                                            ,@CreatedBy
                                            ,@CreatedDate
                                            ,@ModifiedById
                                            ,@ModifiedBy
                                            ,@ModifiedDate
                                            ,@IsActive
                                        );";

                                connection.Execute(insertOption, new
                                {
                                    QuestionId = questionId,
                                    option.OptionText,
                                    option.SortOrder,
                                    CreatedById = loggedinUserId,
                                    CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName,
                                    CreatedDate = DateTime.UtcNow,
                                    ModifiedById = loggedinUserId,
                                    ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName,
                                    ModifiedDate = DateTime.UtcNow,
                                    IsActive = true
                                });

                            }
                        }

                        #endregion

                        connection.Close();
                    }

                }

                return StatusCode(200, new
                {
                    StatusCode = 200
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

        [HttpGet("GetSurvey/{surveyId}")] // Getting Survey Data
        public async Task<IActionResult> GetSurvey(int surveyId)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    var survey = await connection.QueryFirstOrDefaultAsync<SurveyDto>(
                        "SELECT * FROM Surveys WHERE Id = @SurveyId AND IsActive = 1", new { SurveyId = surveyId });

                    if (survey == null)
                    {
                        return StatusCode(404, new
                        {
                            StatusCode = 404
                        });
                    }

                    var employees = await connection.QueryAsync<EmployeeDto>(
                        "SELECT * FROM SurveyEmployees WHERE SurveyId = @SurveyId AND IsActive = 1", new { SurveyId = surveyId });

                    survey.Employees = employees.ToList();

                    var questions = await connection.QueryAsync<QuestionDto>(
                        "SELECT * FROM SurveyQuestions WHERE SurveyId = @SurveyId AND IsActive = 1", new { SurveyId = surveyId });

                    foreach (var question in questions)
                    {
                        var options = await connection.QueryAsync<QuestionOptionDto>(
                            "SELECT * FROM SurveyQuestionOptions WHERE QuestionID = @QuestionID AND IsActive = 1", new { QuestionID = question.Id });
                        question.Options = options.ToList();
                    }

                    survey.Questions = questions.ToList();

                    connection.Close();

                    return StatusCode(200, new
                    {
                        StatusCode = 200,
                        Survey = survey
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

        [HttpPost("GetSurveys")]
        public async Task<IActionResult> GetSurveys(SurveyFilter filters)
        {
            try
            {
                DataTableResponce dtr = new DataTableResponce();
                var tuple = await FilterSurveysData(filters);
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

        private async Task<Tuple<List<Models.Survey>, int>> FilterSurveysData(SurveyFilter search)
        {
            //var timezoneoffset = Convert.ToInt32(HttpContext.Current.Request.Cookies["timeZoneOffsetCookie"].Value);
            try
            {
                var sqlParams = new List<SqlParameter>();
                var sqlParams1 = new List<SqlParameter>();

                var whereClause = $" Where (S.IsActive = 1)";

                if (!string.IsNullOrEmpty(search.Name))
                {
                    whereClause += "\n AND S.Name like '%'+@Name+'%'";
                    sqlParams.Add(new SqlParameter("@Name", search.Name));
                    sqlParams1.Add(new SqlParameter("@Name", search.Name));
                }
                if (!string.IsNullOrEmpty(search.Status))
                {
                    whereClause += "\n AND S.Status like '%'+@Status+'%'";
                    sqlParams.Add(new SqlParameter("@Status", search.Status));
                    sqlParams1.Add(new SqlParameter("@Status", search.Status));
                }
                if (!string.IsNullOrEmpty(search.PublishDate))
                {
                    search.PublishDate = DateTime.ParseExact(search.PublishDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                    whereClause += "\n AND S.PublishDate >= @PublishDate";
                    sqlParams.Add(new SqlParameter("@PublishDate", search.PublishDate));
                    sqlParams1.Add(new SqlParameter("@PublishDate", search.PublishDate));

                }
                if (!string.IsNullOrEmpty(search.CompletionDate))
                {
                    search.CompletionDate = DateTime.ParseExact(search.CompletionDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                    whereClause += "\n AND S.CompletionDate <= @CompletionDate";
                    sqlParams.Add(new SqlParameter("@CompletionDate", search.CompletionDate));
                    sqlParams1.Add(new SqlParameter("@CompletionDate", search.CompletionDate));

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
                        if (search.SortCol.Equals("Name"))
                        {
                            orderByClause = "\n ORDER BY S.Name asc";
                        }
                        else if (search.SortCol.Equals("Status"))
                        {
                            orderByClause = "\n ORDER BY S.Status asc";

                        }
                        else if (search.SortCol.Equals("PublishDate"))
                        {
                            orderByClause = "\n ORDER BY S.PublishDate asc";
                        }
                        else if (search.SortCol.Equals("CompletionDate"))
                        {
                            orderByClause = "\n ORDER BY S.CompletionDate asc";
                        }
                        else if (search.SortCol.Equals("CreatedBy"))
                        {
                            orderByClause = "\n ORDER BY S.CreatedBy asc";
                        }
                        else if (search.SortCol.Equals("CreatedDate"))
                        {
                            orderByClause = "\n ORDER BY S.CreatedDate asc";
                        }
                        else if (search.SortCol.Equals("Department"))
                        {
                            orderByClause = "\n ORDER BY S.Department asc";
                        }
                        else if (search.SortCol.Equals("Site"))
                        {
                            orderByClause = "\n ORDER BY S.Site asc";
                        }
                    }
                    if (search.sSortDir_0.Equals("desc"))
                    {
                        if (search.SortCol.Equals("Name"))
                        {
                            orderByClause = "\n ORDER BY S.Name desc";
                        }
                        else if (search.SortCol.Equals("Status"))
                        {
                            orderByClause = "\n ORDER BY S.Status desc";

                        }
                        else if (search.SortCol.Equals("PublishDate"))
                        {
                            orderByClause = "\n ORDER BY S.PublishDate desc";
                        }
                        else if (search.SortCol.Equals("CompletionDate"))
                        {
                            orderByClause = "\n ORDER BY S.CompletionDate desc";
                        }
                        else if (search.SortCol.Equals("CreatedBy"))
                        {
                            orderByClause = "\n ORDER BY S.CreatedBy desc";
                        }
                        else if (search.SortCol.Equals("CreatedDate"))
                        {
                            orderByClause = "\n ORDER BY S.CreatedDate desc";
                        }
                        else if (search.SortCol.Equals("Department"))
                        {
                            orderByClause = "\n ORDER BY S.Department desc";
                        }
                        else if (search.SortCol.Equals("Site"))
                        {
                            orderByClause = "\n ORDER BY S.Site desc";
                        }
                    }
                }

                var joins = @"";

                var sqlForCount = @"SELECT COUNT(S.Id) FROM dbo.Surveys S "
                   + joins
                   + whereClause;

                using var connection = _context.CreateConnection();
                connection.Open();
                var totalCount = await connection.QuerySingleAsync<int>(sqlForCount, search);

                var sql = @"SELECT S.*
                            FROM dbo.Surveys S"
                            + joins
                            + whereClause
                            + orderByClause
                            + "\n OFFSET @iDisplayStart ROWS FETCH NEXT @iDisplayLength ROWS ONLY;";

                var data = await connection.QueryAsync<Models.Survey>(sql, search);
                var SurveyList = data.ToList<Models.Survey>();
                return new Tuple<List<Models.Survey>, int>(SurveyList, totalCount);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                throw ex;
            }
        }

        [HttpPost("DeleteSurveyById")]
        public async Task<ActionResult> DeleteSurveyById(string Id)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var sitequery = @"Update Surveys SET
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

        [HttpPost("GetEmployeeSurveys")]
        public async Task<IActionResult> GetEmployeeSurveys(SurveyFilter filters)
        {
            try
            {
                DataTableResponce dtr = new DataTableResponce();
                var tuple = await FilterEmployeeSurveysData(filters);
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

        private async Task<Tuple<List<SurveyDto>, int>> FilterEmployeeSurveysData(SurveyFilter search)
        {
            try
            {
                using var connection = _context.CreateConnection();
                connection.Open();

                // If EmployeeCode is not provided in the search filter, fetch from session
                int? employeeCode = search.EmployeeCode;
                if (employeeCode == null || employeeCode == 0)
                {
                    var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    var employeeCodeQuery = "SELECT EmployeeCode FROM Users WHERE UserId = @UserId";
                    employeeCode = await connection.QuerySingleAsync<int>(employeeCodeQuery, new { UserId = loggedinUserId });
                    search.EmployeeCode = employeeCode;
                }

                var whereClause = @" WHERE S.IsActive = 1 
                    AND ES.EmployeeCode = @EmployeeCode
                    -- Stop generating future dates beyond today
                    AND 
	                (
		                S.IsRecurring = 0 
		                OR 
		                (
			                DATEADD(DAY, 0, 
			                CASE 
				                WHEN S.Recursion = 'Weekly' THEN DATEADD(WEEK, N, S.PublishDate)
				                WHEN S.Recursion = 'Monthly' THEN DATEADD(MONTH, N, S.PublishDate)
				                WHEN S.Recursion = 'Quarterly' THEN DATEADD(QUARTER, N, S.PublishDate)
				                WHEN S.Recursion = 'Biannually' THEN DATEADD(MONTH, 6 * N, S.PublishDate)
				                WHEN S.Recursion = 'Annually' THEN DATEADD(YEAR, N, S.PublishDate)
				                ELSE S.PublishDate
			                END) <= GETDATE()
		                )
	                )
                ";

                if (!string.IsNullOrEmpty(search.Name))
                {
                    whereClause += "\n AND S.Name like '%'+@Name+'%'";
                }
                if (!string.IsNullOrEmpty(search.Status))
                {
                    whereClause += "\n AND S.Status like '%'+@Status+'%'";
                }
                if (!string.IsNullOrEmpty(search.PublishDate))
                {
                    search.PublishDate = DateTime.ParseExact(search.PublishDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    whereClause += "\n AND S.PublishDate >= @PublishDate";
                }
                if (!string.IsNullOrEmpty(search.CompletionDate))
                {
                    search.CompletionDate = DateTime.ParseExact(search.CompletionDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    whereClause += "\n AND S.CompletionDate <= @CompletionDate";
                }

                var orderByClause = string.IsNullOrEmpty(search.SortCol) ? "\n ORDER BY RecurringSurveys.ModifiedDate DESC" : $"\n ORDER BY RecurringSurveys.{search.SortCol} {search.sSortDir_0}";

                var joins = @"
                    INNER JOIN SurveyEmployees ES ON S.Id = ES.SurveyId
                    CROSS JOIN NumberSeries ";

                var sql = @$"
                    WITH NumberSeries AS (
                        SELECT TOP 1000 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS N
                        FROM master.dbo.spt_values
                    ),
                    RecurringSurveys AS (
                        SELECT 
                            S.Id, 
                            S.Name, 
                            S.CreatedDate, 
                            S.Recursion, 
                            S.IsRecurring,
                            S.PublishDate, 
                            S.CompletionDate, 
                            S.Status, 
                            S.CreatedBy, 
                            S.Department, 
                            S.Site,
                            S.ModifiedDate,
                            -- Generate Recurring Dates
                            CASE 
                                WHEN S.IsRecurring = 1 AND S.Recursion = 'Weekly' 
                                    THEN DATEADD(WEEK, N, S.PublishDate)
                                WHEN S.IsRecurring = 1 AND S.Recursion = 'Monthly' 
                                    THEN DATEADD(MONTH, N, S.PublishDate)
                                WHEN S.IsRecurring = 1 AND S.Recursion = 'Quarterly' 
                                    THEN DATEADD(QUARTER, N, S.PublishDate)
                                WHEN S.IsRecurring = 1 AND S.Recursion = 'Biannually' 
                                    THEN DATEADD(MONTH, 6 * N, S.PublishDate)
                                WHEN S.IsRecurring = 1 AND S.Recursion = 'Annually' 
                                    THEN DATEADD(YEAR, N, S.PublishDate)
                                ELSE S.PublishDate
                            END AS RecurringDate,
		                    -- Check if a response exists
                            CASE 
                                WHEN EXISTS (
                                    SELECT 1 
                                    FROM SurveyResponses SR 
                                    WHERE SR.SurveyId = S.Id 
                                      AND SR.EmployeeCode = ES.EmployeeCode 
                                      AND CAST(SR.ResponseDate AS DATE) = CAST(
                                            CASE 
                                                WHEN S.IsRecurring = 1 AND S.Recursion = 'Weekly' THEN DATEADD(WEEK, N, S.PublishDate)
                                                WHEN S.IsRecurring = 1 AND S.Recursion = 'Monthly' THEN DATEADD(MONTH, N, S.PublishDate)
                                                WHEN S.IsRecurring = 1 AND S.Recursion = 'Quarterly' THEN DATEADD(QUARTER, N, S.PublishDate)
                                                WHEN S.IsRecurring = 1 AND S.Recursion = 'Biannually' THEN DATEADD(MONTH, 6 * N, S.PublishDate)
                                                WHEN S.IsRecurring = 1 AND S.Recursion = 'Annually' THEN DATEADD(YEAR, N, S.PublishDate)
                                                ELSE S.PublishDate
                                            END AS DATE)
                                ) 
                                THEN 1 ELSE 0 
                            END AS IsSubmitted
                        FROM dbo.Surveys S
                        {joins}
                        {whereClause}
                    )
                    SELECT * FROM RecurringSurveys
                    ORDER BY RecurringDate DESC
                    OFFSET @iDisplayStart ROWS FETCH NEXT @iDisplayLength ROWS ONLY;
                ";

                var data = await connection.QueryAsync<SurveyDto>(sql, search);
                var surveyList = data.ToList();

                return new Tuple<List<SurveyDto>, int>(surveyList, surveyList.Count);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                throw ex;
            }
        }

        [HttpGet("GetSurveyDetails")]
        public async Task<IActionResult> GetSurveyDetails(int surveyId, int? employeeCode, string recurringDate)
        {
            try
            {
                if (!string.IsNullOrEmpty(recurringDate))
                {
                    recurringDate = DateTime.ParseExact(recurringDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                }

                using (var connection = _context.CreateConnection())
                {
                    var surveyQuery = @"
                        SELECT Id, Name, Description, Status, IsRecurring, Recursion, PublishDate, CompletionDate
                        FROM Surveys WHERE Id = @SurveyId AND IsActive = 1;
        
                        SELECT q.Id, q.QuestionText, q.Description, q.QuestionType, q.SortOrder, q.IsRequired, q.MinValue, q.MaxValue, q.Scale, q.Shape, q.Label
                        FROM SurveyQuestions q 
                        WHERE q.SurveyId = @SurveyId AND q.IsActive = 1 ORDER BY q.Id;
        
                        SELECT qo.Id, qo.QuestionID, qo.OptionText
                        FROM SurveyQuestionOptions qo 
                        INNER JOIN SurveyQuestions q ON qo.QuestionID = q.Id 
                        WHERE q.SurveyId = @SurveyId AND qo.IsActive = 1 ORDER BY qo.SortOrder;
        
                        SELECT sr.QuestionId, sr.AnswerText, sr.OptionId, sr.ResponseDate
                        FROM SurveyResponses sr 
                        WHERE sr.SurveyId = @SurveyId AND sr.EmployeeCode = @EmployeeCode; 
        
                        SELECT COUNT(*) FROM SurveyResponses WHERE SurveyId = @SurveyId AND EmployeeCode = @EmployeeCode AND ResponseDate = @ResponseDate;
                    ";

                    using var multi = await connection.QueryMultipleAsync(surveyQuery, new { SurveyId = surveyId, EmployeeCode = employeeCode, ResponseDate = recurringDate });

                    var survey = await multi.ReadFirstOrDefaultAsync();
                    var questions = (await multi.ReadAsync()).ToList();
                    var options = (await multi.ReadAsync()).ToList();
                    var responses = (await multi.ReadAsync()).ToList();

                    if (survey == null)
                    {
                        return StatusCode(404, new
                        {
                            StatusCode = 404
                        });
                    }

                    // Attach options to questions
                    foreach (var question in questions)
                    {
                        question.Options = options.Where(o => o.QuestionID == question.Id).ToList();
                    }

                    connection.Close();

                    return StatusCode(200, new
                    {
                        StatusCode = 200,
                        Survey = survey,
                        Questions = questions,
                        Responses = responses
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

        [HttpPost("SubmitResponses")]
        public async Task<IActionResult> SubmitResponses([FromBody] List<SurveyResponseDto> responses)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                if (responses != null)
                {

                    using (var connection = _context.CreateConnection())
                    {
                        connection.Open();

                        foreach (var response in responses)
                        {
                            if (!string.IsNullOrEmpty(response.ResponseDate))
                            {
                                response.ResponseDate = DateTime.ParseExact(response.ResponseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            }
                            response.CreatedById = loggedinUserId;
                            response.CreatedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                            response.CreatedDate = DateTime.UtcNow;
                            response.ModifiedById = loggedinUserId;
                            response.ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName;
                            response.ModifiedDate = DateTime.UtcNow;
                            response.IsActive = true;

                            string insertQuery = @"INSERT INTO SurveyResponses 
                                        (
                                             SurveyId
                                            ,EmployeeCode
                                            ,QuestionId
                                            ,AnswerText
                                            ,OptionId
                                            ,ResponseDate
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
                                             @SurveyId
                                            ,@EmployeeCode
                                            ,@QuestionId
                                            ,@AnswerText
                                            ,@OptionId
                                            ,@ResponseDate
                                            ,@CreatedById
                                            ,@CreatedBy
                                            ,@CreatedDate
                                            ,@ModifiedById
                                            ,@ModifiedBy
                                            ,@ModifiedDate
                                            ,@IsActive
                            );";

                            await connection.ExecuteAsync(insertQuery, response);
                        }

                        connection.Close();
                    }

                }

                return StatusCode(200, new
                {
                    StatusCode = 200
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

    // DTO Classes
    public class SurveyResponseDto
    {
        public int SurveyId { get; set; }
        public int? EmployeeCode { get; set; }
        public int QuestionId { get; set; }
        public string? AnswerText { get; set; }
        public int? OptionId { get; set; }
        public string? ResponseDate { get; set; }
        public int? CreatedById { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedById { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class SurveyDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public bool? IsRecurring { get; set; }
        public string? Recursion { get; set; }
        public string? PublishDate { get; set; }
        public string? CompletionDate { get; set; }
        public int? SiteId { get; set; }
        public string? Site { get; set; }
        public int? DepartmentId { get; set; }
        public string? Department { get; set; }
        public int? CreatedById { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedById { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? RecurringDate { get; set; }
        public bool? IsSubmitted { get; set; }
        public List<EmployeeDto> Employees { get; set; }
        public List<QuestionDto> Questions { get; set; }
    }

    public class EmployeeDto
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }  // Foreign key reference to Surveys
        public int? EmployeeCode { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int CreatedById { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedById { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }


    public class QuestionDto
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public string? QuestionText { get; set; }
        public string? Description { get; set; }
        public string? QuestionType { get; set; } // Single Select, Multi Select, etc.
        public bool IsRequired { get; set; }
        public string? SortOrder { get; set; } // Ascending, Descending
        public int? MinValue { get; set; } // For Slider
        public int? MaxValue { get; set; } // For Slider
        public int? Scale { get; set; } // For Rating (1-10)
        public string? Shape { get; set; } // Star, Smiley, Heart, Thumb
        public string? Label { get; set; } // For Slider & Rating Labels
        public int? Weight { get; set; } // For Rating Weight
        public int CreatedById { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedById { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public List<QuestionOptionDto> Options { get; set; }
    }

    public class QuestionOptionDto
    {
        public int Id { get; set; }
        public int QuestionID { get; set; }
        public string? OptionText { get; set; }
        public int? SortOrder { get; set; } // To order options
        public int CreatedById { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedById { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }

}
