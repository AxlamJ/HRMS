using System;
using Newtonsoft.Json;
namespace HrManagement.Models
{

    public class User
    {
        [JsonProperty("userId")]
        public int? UserId { get; set; }

        [JsonProperty("employeeCode")]
        public int? EmployeeCode { get; set; }

        [JsonProperty("profilePhotoUrl")]
        public string? ProfilePhotoUrl { get; set; }

        [JsonProperty("userName")]
        public string? UserName { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonProperty("password")]
        public string? Password { get; set; }

        [JsonProperty("passwordHash")]
        public string? PasswordHash { get; set; }

        [JsonProperty("salt")]
        public string? Salt { get; set; }

        [JsonProperty("firstName")]
        public string? FirstName { get; set; }

        [JsonProperty("lastName")]
        public string? LastName { get; set; }

        [JsonProperty("gender")]
        public string? Gender { get; set; }

        [JsonProperty("departmentId")]
        public string? DepartmentId { get; set; }

        [JsonProperty("departmentName")]
        public string? DepartmentName { get; set; }

        [JsonProperty("departmentSubCategoryName")]
        public string? DepartmentSubCategoryName { get; set; }

        [JsonProperty("siteId")]
        public string? SiteId { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; } = true;

        [JsonProperty("createdById")]
        public int? CreatedById { get; set; }

        [JsonProperty("createdBy")]
        public string? CreatedBy { get; set; }

        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty("modifiedById")]
        public int? ModifiedById { get; set; }

        [JsonProperty("modifiedBy")]
        public string? ModifiedBy { get; set; }

        [JsonProperty("modifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        [JsonProperty("lastLoginDate")]
        public DateTime? LastLoginDate { get; set; }

        [JsonProperty("userSites")]
        public string? UserSites { get; set; }

        [JsonProperty("userRoles")]
        public string? UserRoles { get; set; }

        [JsonProperty("roleName")]
        public string? RoleName { get; set; }

        [JsonProperty("roleResources")]
        public string? RoleResources { get; set; }
    }

    public class Roles
    {
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class UsersFilter : FilterBase
    {
        [JsonProperty("firstName")]
        public string? FirstName { get; set; }

        [JsonProperty("lastName")]
        public string? LastName { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }

    }
}
