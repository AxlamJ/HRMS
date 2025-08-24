
using Dapper;
using HrManagement.Data;
using HrManagement.Dto;
using HrManagement.IRepository;
using HrManagement.Models;


namespace HrManagement.Repository
{
    public class TrainingUserProgressReportRepository : ITrainingUserProgressReportRepository
    {
        private readonly DataContext _context;

        public TrainingUserProgressReportRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<string> GetTrainingUserProgressReport(int trainingId)
        {
            using var connection = _context.CreateConnection();

            string sqlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SqlQueries", "UserProgressReports", "UserProgressReport.sql");
            string getQuery = await File.ReadAllTextAsync(sqlFilePath);

            var jsonResult = await connection.QueryFirstOrDefaultAsync<string>(
     getQuery,
     new { TrainingId = trainingId }
 );
            return jsonResult ?? "{}";  // Return empty JSON if null
        }

        // Your existing QueryPart1() and QueryPart2() string methods remain the same.


        public string QueryPart1()
        {
            const string sqlPart1 = @"
SELECT
    t.TrainingID,
    t.Title AS TrainingTitle,
    ts.TrainingStructureId,
    ts.Title AS StructureTitle,
    ts.Type AS StructureType,
    tsc.Id AS TrainingStructureCategoryId,
    tsc.Title AS CategoryTitle,
    ulp.ProgressID,
    ulp.UserID AS LessonUserID,
    ulp.LessonID,
    ulp.Watched,
    ulp.WatchedAt
FROM Trainings t
LEFT JOIN TrainingStructure ts ON ts.TrainingId = t.TrainingID
LEFT JOIN TrainingStructureCategory tsc ON tsc.TrainingStructureId = ts.TrainingStructureId
LEFT JOIN UserLessonProgress ulp ON ulp.LessonID = tsc.Id
WHERE t.IsActive = 1 AND t.TrainingId = @TrainingId
ORDER BY t.TrainingID, ts.TrainingStructureId, tsc.Id;";

            return sqlPart1;

        }

        public string QueryPart2()
        {
            const string sqlPart2 = @"
SELECT
    t.TrainingID,
    tsc.Id AS TrainingStructureCategoryId,
    a.Id AS AssessmentID,
    a.Title AS QuizTitle,
    a.PassingGrade AS QuizPassScore,
    aq.Id AS AssessmentQuestionId,
    aa.Id AS AssessmentAnswerId,
    aa.IsCorrect AS AnswerIsCorrect,
    uqa.AttemptID,
    uqa.UserID AS QuizUserID,
    uqa.Score AS QuizScore,
    uqa.Passed AS QuizPassed,
    uqa.PassScore AS AttemptPassScore,
    uqa.AttemptDate,
    uqan.UserAnswerID,
    uqan.SelectedAnswerID,
    uqan.IsCorrect AS UserAnswerIsCorrect
FROM Trainings t
LEFT JOIN TrainingStructure ts ON ts.TrainingId = t.TrainingID
LEFT JOIN TrainingStructureCategory tsc ON tsc.TrainingStructureId = ts.TrainingStructureId
LEFT JOIN Assessment a ON a.CategoryId = tsc.Id
LEFT JOIN UserQuizAttempt uqa ON uqa.QuizID = a.Id
LEFT JOIN AssessmentQuestion aq ON aq.CategoryId = a.Id
LEFT JOIN AssessmentAnswer aa ON aa.AssessmentQuestionID = aq.Id
LEFT JOIN UserQuestionAnswer uqan ON uqan.AttemptID = uqa.AttemptID
    AND uqan.QuestionID = aq.Id
    AND uqan.SelectedAnswerID = aa.Id
WHERE t.IsActive = 1 AND t.TrainingId = @TrainingId
ORDER BY t.TrainingID, tsc.Id, a.Id, aq.Id, aa.Id;";

            return sqlPart2;
        }

    }
}

