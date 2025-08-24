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
    public class ChatController : ControllerBase
    {

        private readonly ITrainingCommentRepository _chat;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatController(ITrainingCommentRepository chat, IHttpContextAccessor httpContextAccessor)
        {
            _chat = chat;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("WriteComments")]
        public async Task<IActionResult> WriteComments(TrainingComment Comment)
        {
            try
            {
                var userIdString = _httpContextAccessor.HttpContext.Session.GetString("UserId");
                if (!int.TryParse(userIdString, out int loggedInUserId))
                    return Unauthorized();
                var loggedInUserFirstName = _httpContextAccessor.HttpContext.Session.GetString("FirstName");
                var loggedInUserLastName = _httpContextAccessor.HttpContext.Session.GetString("LastName");
                var fullName = $"{loggedInUserFirstName} {loggedInUserLastName}";

                if (Comment.CommentId == 0)
                {
                    Comment.CreatedAt = DateTime.UtcNow;
                    Comment.CreatedBy = fullName;
                    Comment.IsActive = true;
                    Comment.UserId = int.Parse(userIdString);
                }
                else
                {
                    Comment.UpdatedAt = DateTime.UtcNow;
                    Comment.UserId = int.Parse(userIdString);
                }

                var id = await _chat.AddCommentAsync(Comment);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Permission saved successfully.",
                    Data = new { Id = 1 }
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }

        [HttpPost("loadUserComments")]
        public async Task<IActionResult> loadUserComments(int Id)
        {
            try
            {
                var userIdString = _httpContextAccessor.HttpContext.Session.GetString("UserId");
                if (!int.TryParse(userIdString, out int loggedInUserId))
                    return Unauthorized();
                var loggedInUserFirstName = _httpContextAccessor.HttpContext.Session.GetString("FirstName");
                var loggedInUserLastName = _httpContextAccessor.HttpContext.Session.GetString("LastName");
                var fullName = $"{loggedInUserFirstName} {loggedInUserLastName}";

                int u_Id= int.Parse(userIdString);
                var comments = await _chat.GetCommentsByCategoryAsync(Id, u_Id);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Permission saved successfully.",
                    Data = comments
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error." });
            }
        }

    }
}
