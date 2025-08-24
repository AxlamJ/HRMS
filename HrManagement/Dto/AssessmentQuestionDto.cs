namespace HrManagement.Dto
{
    public class AssessmentQuestionDto
    {
        public int? QuestionId { get; set; }
        public string? QuestionTitle { get; set; }
        public int? QuestionUserId { get; set; }
        public int? QuestionCategoryId { get; set; }
        public bool? QuestionIsActive { get; set; }
        public string? QuestionType { get; set; }

        public List<AssessmentAnswerDto>? Answers { get; set; } = new();
    }

}
