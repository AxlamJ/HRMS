using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class EmployeeLevels
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("employeeLevel")]
        public string? EmployeeLevel { get; set; }
    }
}
