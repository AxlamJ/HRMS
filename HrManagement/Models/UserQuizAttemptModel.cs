namespace HrManagement.Models
{
    public class UserQuizAttemptModel
    {
        public int? AttemptID { get; set; }
        public int? UserID { get; set; }
        public int? QuizID { get; set; }
        public DateTime? AttemptDate { get; set; }
        public decimal? Score { get; set; }
        public decimal? PassScore { get; set; }
        public bool? Passed { get; set; }

        public string? ApprovedBy { get; set; }
        public int? CreatedById { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedById { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? Status { get; set; }

        // List of answers for this attempt
        public List<UserQuestionAnswerModel>? UserQuestionAnswers { get; set; }
    }
}
