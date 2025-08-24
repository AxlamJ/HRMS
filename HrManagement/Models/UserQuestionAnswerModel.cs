namespace HrManagement.Models
{
    public class UserQuestionAnswerModel
    {
        public int? UserAnswerID { get; set; }
        public int? AttemptID { get; set; }
        public DateTime? AttemptDate { get; set; }
        public int? QuestionID { get; set; }
        public int? SelectedAnswerID { get; set; }
        public int? UserID { get; set; }
        public bool? IsCorrect { get; set; }

        public string? ApprovedBy { get; set; }
        public int? CreatedById { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedById { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? Status { get; set; }
    }
}
