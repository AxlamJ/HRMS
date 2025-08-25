using HrManagement.Models;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace HrManagement.Dto
{
    public class TrainingCombinedModel
    {
        // Training fields
        public int TrainingId { get; set; }
        public string? ApprovedBy { get; set; }
        public bool? IsActive { get; set; }
        public int? SItemType { get; set; }

        // TrainingsDetails fields
        public int TrainingsDetailId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ProductThumbnail { get; set; }
        public string? InstructorHeading { get; set; }
        public string? Headshot { get; set; }
        public string? InstructorName { get; set; }
        public string? InstructorTitle { get; set; }
        public string? InstructorBio { get; set; }
        public string? LogoImage { get; set; }
        public int? DetailApprovedBy { get; set; }
        public int? CreatedById { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedById { get; set; }
        public string? ModifiedBy { get; set; }
        public string? FilePath { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Status { get; set; }
        public string? Duration { get; set; }
        public int? UserId { get; set; }
        public bool? IsApproved { get; set; }
        public int? RefID { get; set; }
        public string? VisibleTo { get; set; } = string.Empty;
        public string? Departments { get; set; }
        public string? DepartmentsSubCategories { get; set; }
        public string? Employees { get; set; }
        public string? Sites { get; set; }
        public bool? IsExternal { get; set; }
        public string? Url { get; set; }
        public bool? PaymentType { get; set; }
        public decimal? Amount { get; set; }
    }

    public class TrainingsDetailFilter : FilterBase
    {
        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("instructorHeading")]
        public string? InstructorHeading { get; set; }

        [JsonProperty("instructorName")]
        public string? InstructorName { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }
    }
}
