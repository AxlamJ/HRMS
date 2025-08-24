namespace HrManagement.Models
{
    public class TrainingPermission
    {
        public int PermissionId { get; set; }
        public int? UserId { get; set; }
        public byte? ItemType { get; set; }  // 1=Training, 2=Structure, 3=Category
        public int? ItemId { get; set; }
        public bool? IsAssigned { get; set; }
        public int? ApprovedBy { get; set; }
        public int? CreatedById { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedById { get; set; }
        public string? ModifiedBy { get; set; }
        public string? AllowedRole { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? StartDate { get; set; } 
        public DateTime? ExpiryDate { get; set; }

        public TrainingPermission()
        {
            StartDate = DateTime.UtcNow;
            ExpiryDate = DateTime.UtcNow.AddDays(30);
        }
    }
}

