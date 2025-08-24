using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class Survey
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } = "Active"; // Default status

        [JsonProperty("isRecurring")]
        public bool? IsRecurring { get; set; }

        [JsonProperty("recursion")]
        public string? Recursion { get; set; }

        [JsonProperty("publishDate")]
        public DateTime? PublishDate { get; set; }

        [JsonProperty("completionDate")]
        public DateTime? CompletionDate { get; set; }

        [JsonProperty("siteId")]
        public int? SiteId { get; set; }

        [JsonProperty("site")]
        public string? Site { get; set; }

        [JsonProperty("departmentId")]
        public int? DepartmentId { get; set; }

        [JsonProperty("department")]
        public string? Department { get; set; }

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
    }

    public class SurveyFilter : FilterBase
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("publishDate")]
        public string? PublishDate { get; set; }

        [JsonProperty("completionDate")]
        public string? CompletionDate { get; set; }
        [JsonProperty("employeeCode")]
        public int? EmployeeCode { get; set; }

    }

}
