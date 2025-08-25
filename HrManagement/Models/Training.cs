using Newtonsoft.Json;

namespace HrManagement.Models
{
    public class Training
    {
        [JsonProperty("TrainingId")]
        public int TrainingId { get; set; }
        [JsonProperty("Title")]
        public string? Title { get; set; }
        [JsonProperty("UserId")]
        public int? UserId { get; set; }
        [JsonProperty("ApprovedBy")]
        public string? ApprovedBy { get; set; }
        [JsonProperty("CreatedById")]
        public int? CreatedById { get; set; }
        [JsonProperty("CreatedBy")]
        public string? CreatedBy { get; set; }
        [JsonProperty("CreatedDate")]
        public DateTime? CreatedDate { get; set; }
        [JsonProperty("ModifiedById")]
        public int? ModifiedById { get; set; }
        [JsonProperty("ModifiedBy")]
        public string? ModifiedBy { get; set; }
        [JsonProperty("ModifiedDate")]
        public DateTime? ModifiedDate { get; set; }
        [JsonProperty("IsActive")]
        public bool? IsActive { get; set; }
        [JsonProperty("Status")]
        public string? Status { get; set; }
        [JsonProperty("Duration")]
        public int? Duration { get; set; }
        [JsonProperty("IsApproved")]
        public bool? IsApproved { get; set; }= false;
        [JsonProperty("IsExternal")]
        public bool? IsExternal { get; set; }= false;
        [JsonProperty("Url")]
        public string? Url { get; set; }
    }

}
