using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class EmployeeSchedule
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("employeeCode")]
        public int? EmployeeCode { get; set; }

        [JsonProperty("employeeName")]
        public string? EmployeeName { get; set; }

        [JsonProperty("siteId")]
        public int? SiteId { get; set; }

        [JsonProperty("siteName")]
        public string? SiteName { get; set; }

        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("startTime")]
        public string? StartTime { get; set; }

        [JsonProperty("endTime")]
        public string? EndTime { get; set; }

        [JsonProperty("onDays")]
        public string? OnDays { get; set; }

        [JsonProperty("offDays")]
        public string? OffDays { get; set; }

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
        public bool IsActive { get; set; } = true;

        [JsonProperty("isActiveApproved")]
        public bool IsActiveApproved { get; set; } = true;
    }


    public class EmployeeScheduleRoster
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("rosterId")]
        public string? RosterId { get; set; }

        [JsonProperty("employeeCode")]
        public int? EmployeeCode { get; set; }

        [JsonProperty("employeeName")]
        public string? EmployeeName { get; set; }

        [JsonProperty("siteId")]
        public int? SiteId { get; set; }

        [JsonProperty("siteName")]
        public string? SiteName { get; set; }

        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

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
        public bool IsActive { get; set; } = true;
    }

    public class ScheduleFilters
    {

        [JsonProperty("employeeCode")]
        public int? EmployeeCode { get; set; }

        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("siteId")]
        public int? SiteId { get; set; }

    }

    public class ManageScheduleFilters :FilterBase
    {

        [JsonProperty("employeeCode")]
        public List<int>? EmployeeCode { get; set; }

        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }
    }
}
