namespace HrManagement.Dto
{
    public class AssessmentDto
    {
        public int AssessmentId { get; set; }
        public string? AssessmentTitle { get; set; }
        public decimal? AssessmentPassingGrade { get; set; }
        public bool? AssessmentIsPassingGrade { get; set; }
        public int? AssessmentUserId { get; set; }
        public int? AssessmentCategoryId { get; set; }
        public bool? AssessmentIsActive { get; set; }
        public string? AssessmentStatus { get; set; }
        public string? AssessmentPassMessage { get; set; }
        public string? AssessmentFailMessage { get; set; }

        public List<AssessmentQuestionDto>? Questions { get; set; } = new();
        public List<FileMediaUplaodDto>? FileMediaUplaod { get; set; } = new();
    }

}
