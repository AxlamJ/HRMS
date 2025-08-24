namespace HrManagement.Dto
{
    public class AssessmentAnswerDto
    {
        public int? AnswerId { get; set; }
        public string? AnswerTitle { get; set; }
        public bool? IsCorrect { get; set; }
        public int? AnswerUserId { get; set; }
        public int? AnswerCategoryId { get; set; }
        public int? AssessmentQuestionId { get; set; }
        public bool? AnswerIsActive { get; set; }
        public string? AnswerType { get; set; }
    }

}
