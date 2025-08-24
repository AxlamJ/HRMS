using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class PermissionRoles
    {
        [JsonProperty("roleId")]
        public int? RoleId { get; set; }

        [JsonProperty("roleName")]
        public string? RoleName { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("roleResources")]
        public string? RoleResources { get; set; }

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

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("isActiveApproved")]
        public bool IsActiveApproved { get; set; }
    }

    public class PermissionRolesFilters : FilterBase
    {
        [JsonProperty("roleName")]
        public string? RoleName { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

    }

    public class UserRoles
    {
        [JsonProperty("userId")]
        public int? UserId { get; set; }

        [JsonProperty("roleId")]
        public string? RoleId { get; set; }

        [JsonProperty("modifiedById")]
        public int? ModifiedById { get; set; }

        [JsonProperty("modifiedBy")]
        public string? ModifiedBy { get; set; }

        [JsonProperty("modifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
    }
}
