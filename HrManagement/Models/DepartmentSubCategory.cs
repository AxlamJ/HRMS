using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class DepartmentSubCategory
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("subCategoryName")]
        public string? SubCategoryName { get; set; }

        [JsonProperty("departmentId")]
        public int? DepartmentId { get; set; }

        [JsonProperty("departmentName")]
        public string? DepartmentName { get; set; }

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
        public bool? IsActive { get; set; }

        [JsonProperty("isActiveApproved")]
        public bool? IsActiveApproved { get; set; }
    }

    public class DepartmentsSubCategoriesFilter : FilterBase
    {
        [JsonProperty("departmentId")]
        public int? DepartmentId { get; set; }

        [JsonProperty("subCategoryName")]
        public string? SubCategoryName { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }

    }
}
