namespace HrManagement.Dto
{
    public class UploadMediaDto
    {
        public IFormFile? File { get; set; }
        public int Id { get; set; }
        public int ReferenceId { get; set; }
        public string? ModuleName { get; set; }
        public string? FolderName { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public string? Title { get; set; }
    }
}
