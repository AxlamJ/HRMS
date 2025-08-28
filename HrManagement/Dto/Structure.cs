namespace HrManagement.Dto
{
    public class Courses
    {
        public int? CourseId { get; set; }
        public int? SemesterId { get; set; }
        public string? CourseTitle { get; set; }
        public string? CourseType { get; set; }


    }

    public class Structure
    {
        public int? SemesterId { get; set; }
        public string? SemesterTitle { get; set; }
        public string? TrainingId { get; set; }
        public string? SemesterType { get; set; }
    }
}
