namespace HrManagement.Models
{
    using Newtonsoft.Json;
    using System;

    public class TrainingsDetail
    {
        [JsonProperty("trainingsDetailId")]
        public int TrainingsDetailId { get; set; }  // Primary key, 0 for new product

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("trainingId")]
        public int? TrainingId { get; set; }

        [JsonProperty("duration")]
        public int? Duration { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("instructorHeading")]
        public string? InstructorHeading { get; set; }

        [JsonProperty("instructorName")]
        public string? InstructorName { get; set; }

        [JsonProperty("instructorTitle")]
        public string? InstructorTitle { get; set; }

        [JsonProperty("instructorBio")]
        public string? InstructorBio { get; set; }

        // Audit fields
        [JsonProperty("createdById")]
        public int CreatedById { get; set; }

        [JsonProperty("createdBy")]
        public string? CreatedBy { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("modifiedById")]
        public int ModifiedById { get; set; }

        [JsonProperty("modifiedBy")]
        public string? ModifiedBy { get; set; }

        [JsonProperty("modifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
    }

}
