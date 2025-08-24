using System.Text.Json.Serialization;

public class TrainingLessonSession
{
    public int TrainingId { get; set; }
    public string? TrainingTitle { get; set; }
    public bool? TrainingIsActive { get; set; }
    public List<TrainingStructureLessonSession> Structures { get; set; } = new();
}

public class TrainingStructureLessonSession
{
    public int TrainingStructureId { get; set; }
    public string? StructureTitle { get; set; }
    public string? StructureType { get; set; }
    public string? SStatus { get; set; }
    public List<TrainingStructureCategoryLessonSession> Categories { get; set; } = new();


}

public class TrainingStructureCategoryLessonSession
{
    public int CategoryId { get; set; }

    public string? CategoryTitle { get; set; }

    public int CategoryUserId { get; set; }

    public int CategoryTrainingStructureId { get; set; }

    public int? CategoryApprovedBy { get; set; }

    public string? CategoryType { get; set; }
    public int? CategorySubCategoryId { get; set; }
    public string? CStatus { get; set; }

    public List<DtosTrainingSubCategoryLessonSession>? TrainingSubCategories { get; set; } = new();


}


public class DtosTrainingSubCategoryLessonSession
{
    public int CategoryId { get; set; }

    public string? CategoryTitle { get; set; }

    public int CategoryUserId { get; set; }

    public int CategoryTrainingStructureId { get; set; }

    public int? CategoryApprovedBy { get; set; }

    public string? CategoryStatus { get; set; }
    public string? CategoryType { get; set; }
    public int? CategorySubCategoryId { get; set; }
    public string? CStatus { get; set; }
}