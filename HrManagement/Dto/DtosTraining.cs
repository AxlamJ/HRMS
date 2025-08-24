using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace HrManagement.Dto
{
    using System.Text.Json.Serialization;

    public class DtosTrainingCategory
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("trainingStructureId")]
        public int TrainingStructureId { get; set; }

        [JsonPropertyName("approvedBy")]
        public int? ApprovedBy { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("subCategoryId")]
        public int? SubCategoryId { get; set; }

        [JsonPropertyName("cItemType")]
        public int? CItemType { get; set; }

        public List <DtosTrainingSubCategory> TrainingSubCategories { get; set; }= new List<DtosTrainingSubCategory>(); 
    }

    public class DtosTrainingSubCategory
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("trainingStructureId")]
        public int TrainingStructureId { get; set; }

        [JsonPropertyName("approvedBy")]
        public int? ApprovedBy { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("subCategoryId")]
        public int? SubCategoryId { get; set; }

        [JsonPropertyName("cItemType")]
        public int? CItemType { get; set; }
    }


    public class DtosTrainingStructure
    {
        [JsonPropertyName("trainingStructureId")]
        public int TrainingStructureId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("sItemType")]
        public int? SItemType { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("approvedBy")]
        public int? ApprovedBy { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("trainingId")]
        public int TrainingId { get; set; }

      

        [JsonPropertyName("categories")]
        public List<DtosTrainingCategory> Categories { get; set; } = new();
    }

    public class DtosTrainingsDetail
    {
        [JsonPropertyName("trainingsDetailId")]
        public int TrainingsDetailId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("instructorHeading")]
        public string InstructorHeading { get; set; }

        [JsonPropertyName("instructorName")]
        public string InstructorName { get; set; }

        [JsonPropertyName("instructorTitle")]
        public string InstructorTitle { get; set; }

        [JsonPropertyName("instructorBio")]
        public string InstructorBio { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("Type")]
        public string? Type { get; set; }

       
    }

    public class DtosTraining
    {
        [JsonPropertyName("trainingId")]
        public int TrainingId { get; set; }

        [JsonPropertyName("tItemType")]
        public int? TItemType { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonProperty("trainingIsActive")]
        public bool? TrainingIsActive { get; set; }
       
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("approvedBy")]
        public int? ApprovedBy { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }
        [JsonPropertyName("isApproved")]
        public bool IsApproved { get; set; }

        [JsonPropertyName("details")]
        public List<DtosTrainingsDetail> Details { get; set; } = new();

        [JsonPropertyName("structures")]
        public List<DtosTrainingStructure> Structures { get; set; } = new();
        public List<FileMediaUplaodDto>? FileMediaUplaod { get; set; } = new();
    }

}
