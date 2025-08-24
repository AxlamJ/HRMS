using HrManagement.Dto;
using HrManagement.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.WebApi
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserProgressReportController : ControllerBase
    {
        private readonly ITrainingUserProgressReportRepository _reportRepository;
        public UserProgressReportController(
            ITrainingUserProgressReportRepository reportRepository
            )
        {
            _reportRepository= reportRepository;
        }
        [HttpPost("GetUserLessonReport")]
        public async Task<IActionResult> GetUserLessonReport(int? id)
        {
            if (id <= 0||id==null)
                return BadRequest(new { Message = "Invalid categoryId." });
            try
            {
                var assessments = await _reportRepository.GetTrainingUserProgressReport((int)id);

                if (assessments == null || !assessments.Any())
                    return StatusCode(200, new { StatusCode = 200, Message = "Get Assessment successfully.", Data = new AssessmentDto() });

                return StatusCode(200, new
                {
                    StatusCode = 200,
                    Message = "Get Assessment successfully.",
                    Data = assessments
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }
    }
}
