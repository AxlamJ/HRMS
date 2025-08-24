namespace HrManagement.Models
{
    public class FileMediaUpload
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? ContentType { get; set; }
        public long? Size { get; set; }
        public string? ModuleName { get; set; }
        public int? ReferenceId { get; set; }
        public string? Extension { get; set; }
        public int? CreatedById { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedById { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public string? Title { get; set; }
    }

}
