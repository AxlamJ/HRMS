namespace HrManagement.Models
{
    public class UserLessonProgress
    {
        public int? ProgressID { get; set; }
        public int? UserID { get; set; }
        public int? LessonID { get; set; }
        public bool? Watched { get; set; }
        public DateTime? WatchedAt { get; set; }
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
