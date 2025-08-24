using HrManagement.Helpers;
using HrManagement.IRepository;
using HrManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.WebApi
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LessonQuizAttemptController : ControllerBase
    {
        private readonly IUserQuizRepository _userQuizRepository;

        public LessonQuizAttemptController(IUserQuizRepository userQuizRepository)
        {
            _userQuizRepository = userQuizRepository;
        }
        [HttpPost("UpsertUserQuizAttempt")]
        public async Task<IActionResult> UpsertUserQuizAttempt(UserQuizAttemptModel attempt)
        {
            try
            {
              

                var loggedInUserId =  HttpContext.Session.GetInt32("EmployeeCode");
                var loggedInUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedInUserLastName = HttpContext.Session.GetString("LastName");

                var fullName = $"{loggedInUserFirstName} {loggedInUserLastName}";

                attempt.UserID = loggedInUserId;
                attempt.ModifiedById = loggedInUserId;
                attempt.ModifiedBy = fullName;
                attempt.ModifiedDate = DateTime.UtcNow;
                attempt.IsActive ??= true;
                attempt.Status ??= "Completed";

                var id = await _userQuizRepository.UpsertUserQuizAttempt(attempt);


              

               
                return Ok(new { StatusCode = 200, Message = "Quiz attempt saved successfully", Data = new { Id = id } });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error" });
            }
        }

        [HttpPost("UpsertUserLessonProgress")]
        public async Task<IActionResult> UpsertUserLessonProgress(UserLessonProgress progress)
        {
            try
            {
               // var empid = Convert.ToInt32(HttpContext.Session.GetString("EmployeeCode"));

                var loggedInUserId = HttpContext.Session.GetInt32("EmployeeCode");
                var loggedInUserFirstName = HttpContext.Session.GetString("FirstName");
                var loggedInUserLastName = HttpContext.Session.GetString("LastName");

                var fullName = $"{loggedInUserFirstName} {loggedInUserLastName}";

                progress.UserID = loggedInUserId;
                progress.ModifiedById = loggedInUserId;
                progress.ModifiedBy = fullName;
                progress.ModifiedDate = DateTime.UtcNow;
                progress.IsActive = true;
                progress.Watched = true;
                progress.Status ??= "Completed";

                var id = await _userQuizRepository.UpsertUserLessonProgress(progress);

                return Ok(new { StatusCode = 200, Message = "Lesson Completed successfully", Data = new { Id = id } });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error" });
            }
        }

    }
}