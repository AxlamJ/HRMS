using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class EducationDetails
    {

        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("employeeCode")]
        public int? EmployeeCode { get; set; }

        [JsonProperty("collegeUniversity")]
        public string? CollegeUniversity { get; set; }

        [JsonProperty("degree")]
        public string? Degree { get; set; }

        [JsonProperty("major")]
        public string? Major { get; set; }

        [JsonProperty("yearGraduated")]
        public string? YearGraduated { get; set; }

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
}
