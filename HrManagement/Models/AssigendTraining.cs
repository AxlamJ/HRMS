using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class AssignTraining
    {
            [JsonProperty("id")]
            public int? Id { get; set; }
            [JsonProperty("visibleTo")]
            public string? VisibleTo { get; set; }
            [JsonProperty("refID")]
            public int? RefID { get; set; }
            [JsonProperty("departments")]
            public string? Departments { get; set; }
            [JsonProperty("title")]
            public string? title { get; set; }
            [JsonProperty("departmentsSubCategories")]
            public string? DepartmentsSubCategories { get; set; }
            [JsonProperty("employees")]
            public string? Employees { get; set; }
            [JsonProperty("sites")]
            public string? Sites { get; set; }
            [JsonProperty("isActive")]
            public bool? IsActive { get; set; } = true; // Default to true
            [JsonProperty("assigneDate")]
            public DateTime? AssigneDate { get; set; }  // Default to true
        }
}
