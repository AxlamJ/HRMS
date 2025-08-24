namespace HrManagement.Dto
{
    public class TrainingPermissionDto
    {
        public int TrainingId { get; set; } 
        public string? Title { get; set; }  
        public string? ItemType { get; set; }  
        public string? AllowedRole { get; set; }  
        public int? PermissionId { get; set; }  
        public bool? IsAssigned { get; set; }  

    }
}
