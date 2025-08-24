namespace HrManagement.Models
{
    public class TrainingComment
    {
        public int CommentId { get; set; }
        public int? CategoryId { get; set; }  
        public int UserId { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public int Status { get; set; } //1 admin //2 user
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
