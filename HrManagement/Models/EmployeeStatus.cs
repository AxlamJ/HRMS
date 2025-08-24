using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class EmployeeStatus
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("employeeStatusName")]
        public string? EmployeeStatusName { get; set; }

    }
}
