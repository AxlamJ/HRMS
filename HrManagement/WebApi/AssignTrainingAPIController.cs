using Dapper;
using HrManagement.Data;
using HrManagement.Helpers;
using HrManagement.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace HrManagement.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignTrainingAPIController : ControllerBase
    {

        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AssignTrainingAPIController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpPost("UpsertAssignTraining")]
        public async Task<IActionResult> UpsertNewsFeed(AssignTraining news)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                if (news != null)
                {
                   
                        var UpdateQuery = @"UPDATE Trainings SET 
                                                 VisibleTo = @VisibleTo
                                                ,Departments = @Departments
                                                ,DepartmentsSubCategories = @DepartmentsSubCategories
                                                ,Employees = @Employees
                                                ,Sites = @Sites
                                                 WHERE TrainingId = @RefID;";


                      
                        news.IsActive = true;
                        using var connection = _context.CreateConnection();
                        connection.Open();
                        await connection.ExecuteAsync(UpdateQuery, news);
                        connection.Close();
                        return StatusCode(200, new
                        {
                            StatusCode = 200,
                            //Message = "Site created successfully!",
                            //Data = new { Id = productId }
                        });
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


        [HttpPost("GetNewsFeed")]
        public async Task<ActionResult> GetNewsFeed(FilterBase filters)
        {

            try
            {
                var tuple = await FilterNewsListData(filters);
                return Ok(new
                {
                    StatusCode = 200,
                    NewsList = tuple.Item1,
                    TotalRecords = tuple.Item2
                });
            }
            catch (Exception ex)
            {

                ExceptionLogger.LogException(ex);
                throw ex;
            }
        }


        private async Task<Tuple<List<NewsFeed>, int>> FilterNewsListData(FilterBase search)
        {
            //var timezoneoffset = Convert.ToInt32(HttpContext.Current.Request.Cookies["timeZoneOffsetCookie"].Value);
            try
            {
                var sqlParams = new List<SqlParameter>();
                var sqlParams1 = new List<SqlParameter>();

                var whereClause = $" Where N.IsActive = 1";



                var orderByClause = " Order By N.ModifiedDate desc";

                var joins = @"";

                var sqlForCount = @"SELECT COUNT(N.Id) FROM NewsFeed N "
                   + joins
                   + whereClause;


                using var connection = _context.CreateConnection();
                connection.Open();
                var totalCount = await connection.QuerySingleAsync<int>(sqlForCount, search);
                //connection.Close();

                var sql = @"SELECT * FROM NewsFeed N"
                            + joins
                            + whereClause
                            + orderByClause
                            + "\n OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                search.Offset = search.PageNumber * search.PageSize;
                var data = await connection.QueryAsync<NewsFeed>(sql, search);
                var NewsList = data.ToList<NewsFeed>();
                //var data = db.Database.SqlQuery<Sites>(sql, sqlParams.ToArray()).ToList();
                return new Tuple<List<NewsFeed>, int>(NewsList, totalCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("GetNewsbyId")]
        public async Task<ActionResult> GetNewsbyId(int Id)
        {
            try
            {
                var query = "Select * from NewsFeed Where Id = @Id and IsActive = 1";
                var paramas = new { Id = Id };
                using var connection = _context.CreateConnection();
                connection.Open();
                var newsdata = await connection.QueryAsync<NewsFeed>(query, paramas);

                connection.Close();

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    News = newsdata.FirstOrDefault()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }


        [HttpPost("DeleteNewsbyId")]
        public async Task<ActionResult> DeleteNewsbyId(int Id)
        {
            try
            {
                var loggedinUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var loggedinUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedinUserLastName = HttpContext.Session.GetString("LastName");

                var query = @"UPDATE NewsFeed SET
                              ModifiedById = @ModifiedById
                             ,ModifiedBy = @ModifiedBy
                             ,ModifiedDate = @ModifiedDate
                             ,IsActive = @IsActive
                              Where Id = @Id";
                var paramas = new { Id = Id, ModifiedById = loggedinUserId, ModifiedBy = loggedinUserFirstName + " " + loggedinUserLastName, ModifiedDate = DateTime.UtcNow, IsActive = false };
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
                return StatusCode(500, new
                {
                    StatusCode = 500
                });
            }
        }


        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(ImageUploadModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Base64) || string.IsNullOrEmpty(model.FileName))
                    return StatusCode(500, new { StatusCode = HttpStatusCode.InternalServerError, imageUrl = "" });

                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "news-feed");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);



                string imagePath = Path.Combine(uploadsFolder, model.FileName);

                // Remove base64 prefix (e.g., "data:image/png;base64,")
                string base64Data = Regex.Replace(model.Base64, @"^data:image\/[a-zA-Z]+;base64,", "");
                byte[] imageBytes = Convert.FromBase64String(base64Data);

                await System.IO.File.WriteAllBytesAsync(imagePath, imageBytes);


                var relativePath = "/news-feed/" + model.FileName;

                return StatusCode(200, new { StatusCode = HttpStatusCode.OK, imageUrl = relativePath });
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
