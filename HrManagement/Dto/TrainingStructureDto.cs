using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace HrManagement.Dto
{
    public class TrainingStructureDto
    {
        [JsonProperty("trainingStructureId")]
        public int TrainingStructureId { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("Type")]
        public string? Type { get; set; }

        [JsonProperty("userId")]
        public int? UserId { get; set; }

        [JsonProperty("approvedBy")]
        public string? ApprovedBy { get; set; }

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

        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("duration")]
        public int? Duration { get; set; }

        [JsonProperty("trainingId")]
        public int? TrainingId { get; set; }

        [JsonPropertyName("outlines")]
        public List<DtosTrainingOutline> TrainingOutlines { get; set; } = new();

        [JsonPropertyName("fileMediaUplaod")]
        public List<FileMediaUplaodDto>? FileMediaUplaod { get; set; } = new();
    }
}
