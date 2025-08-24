using System.Text.Json.Serialization;

namespace HrManagement.Dto
{
    public class DtosTrainingPost
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("categoryId")]
        public int? CategoryId { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("createdBy")]
        public string? CreatedBy { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string? ModifiedBy { get; set; }

        [JsonPropertyName("modifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        [JsonPropertyName("isActive")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        // Add more fields if needed
    }


    public class DtosTrainingCategoryPost
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("userId")]
        public int? UserId { get; set; }

        [JsonPropertyName("trainingStructureId")]
        public int? TrainingStructureId { get; set; }

        [JsonPropertyName("approvedBy")]
        public string? ApprovedBy { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("isActive")]
        public string? IsActive { get; set; }
        [JsonPropertyName("itemType")]
        public string? ItemType { get; set; }

        [JsonPropertyName("posts")]
        public List<DtosTrainingPost> Posts { get; set; } = new();
        public List<FileMediaUplaodDto>? FileMediaUplaod { get; set; } = new();
    }

}
