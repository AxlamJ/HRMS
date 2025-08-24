using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace HrManagement.Models
{
    public class Assessment
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("imageId")]
        public int ImageId { get; set; }

        [JsonPropertyName("title")]
        public string? title { get; set; }

        [JsonPropertyName("passingGrade")]
        public int? PassingGrade { get; set; }

        [JsonPropertyName("isPassingGrade")]
        public bool? IsPassingGrade { get; set; }

        [JsonPropertyName("userId")]
        public int? UserId { get; set; }

        [JsonPropertyName("categoryId")]
        public int? CategoryId { get; set; }

        [JsonPropertyName("approvedBy")]
        public string? ApprovedBy { get; set; }

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

        [JsonPropertyName("passConfirmationMessage")]
        public string? PassConfirmationMessage { get; set; }

        [JsonPropertyName("failConfirmationMessage")]
        public string? FailConfirmationMessage { get; set; }

        [JsonPropertyName("imageThumbnail")]
        public IFormFile? imageThumbnail { get; set; }

        [JsonPropertyName("questions")]
        [FromForm(Name = "questions")]
        public string? QuestionsJson { get; set; } // Raw JSON string from FormData

        [JsonIgnore]
        public List<AssessmentQuestion> Questions { get; set; } = new();
    }
}
