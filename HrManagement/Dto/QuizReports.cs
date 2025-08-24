namespace HrManagement.Dto
{
    public class QuizReports
    {
        public int? QuizId { get; set; }  
        public int? CourseId { get; set; }  
        public int? EmployeeId { get; set; }  
        public string? QuizTitle { get; set; }  
        public decimal? SecureScore { get; set; }  
        public decimal? PassScore { get; set; }  
        public decimal? TotalScore { get; set; }  
    }
}
