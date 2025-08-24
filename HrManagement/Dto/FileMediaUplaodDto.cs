namespace HrManagement.Dto
{
    public class FileMediaUplaodDto
    {
        public int FileId { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? ContentType { get; set; }
        public long? Size { get; set; }
        public string? ModuleName { get; set; }
        public int? ReferenceId { get; set; }
        public string? Extension { get; set; }
        public string? FileType { get; set; }
        public string? FileTitle { get; set; }
        public string? FileDescription { get; set; }
        public bool? FileIsActive { get; set; }
    }

}
