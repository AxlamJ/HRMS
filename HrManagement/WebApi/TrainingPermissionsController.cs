using DocumentFormat.OpenXml.InkML;
using HrManagement.Helpers;
using HrManagement.IRepository;
using HrManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security;

namespace HrManagement.WebApi
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingPermissionsController : ControllerBase
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TrainingPermissionsController(IPermissionRepository permissionRepository, IHttpContextAccessor httpContextAccessor)
        {
            _permissionRepository = permissionRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("UpsertPermission")]
        public async Task<IActionResult> UpsertPermission([FromBody] List<TrainingPermission> permissionList)
        {
            try
            {
                var userIdString = _httpContextAccessor.HttpContext.Session.GetString("UserId");
                if (!int.TryParse(userIdString, out int loggedInUserId))
                    return Unauthorized();
                var loggedInUserFirstName = _httpContextAccessor.HttpContext.Session.GetString("FirstName");
                var loggedInUserLastName = _httpContextAccessor.HttpContext.Session.GetString("LastName");
                var fullName = $"{loggedInUserFirstName} {loggedInUserLastName}";

                foreach (var permission in permissionList)
                {
                    if (permission.PermissionId == 0)
                    {
                        permission.CreatedById = loggedInUserId;
                        permission.CreatedBy = fullName;
                        permission.CreatedDate = DateTime.UtcNow;
                        permission.IsActive = true;

                        permission.ModifiedById = loggedInUserId;
                        permission.ModifiedBy = fullName;
                        permission.ModifiedDate = DateTime.UtcNow;
                        permission.StartDate ??= DateTime.UtcNow;
                        permission.ExpiryDate ??= DateTime.UtcNow.AddDays(30);
                    }
                    else
                    {
                        permission.ModifiedById = loggedInUserId;
                        permission.ModifiedBy = fullName;
                        permission.ModifiedDate = DateTime.UtcNow;
                        permission.IsActive = true;
                    }

                    var id = await _permissionRepository.UpsertPermissionAsync(permission);
                }
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


        [HttpPost("GetByRoleName")]
        public async Task<ActionResult> GetByRoleName(string RoleName)
        {
            try
            {
                if (RoleName is not null)
                {
                    var roledata = await _permissionRepository.GetPermissionRole(RoleName);
                    return StatusCode(200, new
                    {
                        StatusCode = 200,
                        Data = roledata
                    });
                }
                return BadRequest(new { StatusCode = 400, Message = "Invalid input." });
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
