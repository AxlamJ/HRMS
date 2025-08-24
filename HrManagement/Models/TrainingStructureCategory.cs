using System.Text.Json.Serialization;

namespace HrManagement.Models
{
    public class TrainingStructureCategory
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("userId")]
        public int? UserId { get; set; }

        [JsonPropertyName("trainingStructureId")]
        public int? TrainingStructureId { get; set; }

        [JsonPropertyName("approvedBy")]
        public int? ApprovedBy { get; set; }

        [JsonPropertyName("createdById")]
        public int? CreatedById { get; set; }

        [JsonPropertyName("createdBy")]
        public string? CreatedBy { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonPropertyName("modifiedById")]
        public int? ModifiedById { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string? ModifiedBy { get; set; }

        [JsonPropertyName("modifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        [JsonPropertyName("isActive")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("duration")]
        public int? Duration { get; set; }

        [JsonPropertyName("trainingId")]
        public int? TrainingId { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("subCategoryId")]
        public int? SubCategoryId { get; set; }
    }


}
