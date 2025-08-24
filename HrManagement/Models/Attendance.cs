using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class Attendance
    {
        [JsonProperty("attendanceId")]
        public int? AttendanceId { get; set; }

        [JsonProperty("employeeCode")]
        public int? EmployeeCode { get; set; }

        [JsonProperty("firstName")]
        public string? FirstName { get; set; }

        [JsonProperty("lastName")]
        public string? LastName { get; set; }

        [JsonProperty("attendanceDate")]
        public string? AttendanceDate { get; set; }

        [JsonProperty("timeSheet")]
        public string? TimeSheet { get; set; }

        [JsonProperty("clockIn")]
        public string? ClockIn { get; set; }

        [JsonProperty("clockOut")]
        public string? ClockOut { get; set; }

        [JsonProperty("clockTime")]
        public string? ClockTime { get; set; }

        [JsonProperty("totalBreakTime")]
        public string? TotalBreakTime { get; set; }

        [JsonProperty("totalWorkTime")]
        public string? TotalWorkTime { get; set; }

        [JsonProperty("totalTime")]
        public string? TotalTime { get; set; }

        [JsonProperty("totalOverTime")]
        public string? TotalOverTime { get; set; }

        [JsonProperty("staticRuleMode")]
        public string? StaticRuleMode { get; set; }

        [JsonProperty("abnormalSituation")]
        public string? AbnormalSituation { get; set; }
    }

    public class AttendanceFilters : FilterBase
    {
        [JsonProperty("employeeCode")]
        public int? EmployeeCode { get; set; }

        [JsonProperty("dateFrom")]
        public string? DateFrom { get; set; }

        [JsonProperty("dateTo")]
        public string? DateTo { get; set; }

        [JsonProperty("startDate")]
        public DateOnly? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateOnly? EndDate { get; set; }

    }
}
