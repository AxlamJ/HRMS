using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class TrainingAssignModel
    {
        [JsonProperty("assigneId")]
        public int AssigneId { get; set; }
        [JsonProperty("visibleTo")]
        public string? VisibleTo { get; set; }
        [JsonProperty("departments")]
        public string? Departments { get; set; }
        [JsonProperty("departmentsSubCategories")]
        public string? DepartmentsSubCategories { get; set; }
        [JsonProperty("employees")]
        public string? Employees { get; set; }
       
        [JsonProperty("AssigneDate")]
       
        public DateTime? AssigneDate { get; set; }
        [JsonProperty("title")]
        public string? title { get; set; }
       
        [JsonProperty("sites")]
        public string? Sites { get; set; }
        [JsonProperty("assigneby")]
        public int? Assigneby { get; set; }

    }
}
