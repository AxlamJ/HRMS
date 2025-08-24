using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HrManagement.ApplicationSetting;
using HrManagement.Data;
using HrManagement.Dto;
using HrManagement.IRepository;
using HrManagement.Models;
using Microsoft.Data.SqlClient;

namespace HrManagement.Repository
{
    public class UserQuizRepository : IUserQuizRepository
    {
        private readonly DataContext _context;

        public UserQuizRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<int> UpsertUserQuizAttempt(UserQuizAttemptModel attempt)
        {

            using var connection = _context.CreateConnection();

            var AttemptID = await AlreadyQuizAttempt(attempt.QuizID, attempt.UserID);

            const string sql = "SELECT CategoryId FROM Assessment WHERE Id = @AssessmentId";
            var CateoryId = connection.QueryFirstOrDefault<int?>(sql, new { AssessmentId = attempt.QuizID });

            var progress = new UserLessonProgress();
            progress.UserID = attempt.UserID;
            progress.ModifiedById = attempt.CreatedById;
            progress.ModifiedDate = DateTime.UtcNow;
            progress.IsActive = true;
            progress.Watched = true;
            progress.LessonID = CateoryId;
            progress.Status ??= "Completed";

            var result = UpsertUserLessonProgress(progress);

          
            if (AttemptID == 0 || AttemptID == null)
            {

                attempt.CreatedDate = attempt.ModifiedDate;
                attempt.AttemptDate = attempt.ModifiedDate;
                attempt.CreatedBy = attempt.ModifiedBy;
                const string insertAttempt = @"
                INSERT INTO UserQuizAttempt
                (
                    UserId, QuizID, AttemptDate, Score, PassScore, Passed,
                    ApprovedBy, CreatedById, CreatedBy, CreatedDate,
                    ModifiedById, ModifiedBy, ModifiedDate,
                    IsActive, Status
                )
                VALUES
                (
                    @UserID, @QuizID, @AttemptDate, @Score, @PassScore, @Passed,
                    @ApprovedBy, @CreatedById, @CreatedBy, @CreatedDate,
                    @ModifiedById, @ModifiedBy, @ModifiedDate,
                    @IsActive, @Status
                );
                SELECT CAST(SCOPE_IDENTITY() as int);";
                var attemptId = await connection.ExecuteScalarAsync<int>(insertAttempt, attempt);
               
                
                // Insert answers if available
                if (attempt.UserQuestionAnswers != null && attempt.UserQuestionAnswers.Any())
                {
                    const string insertAnswer = @"
                    INSERT INTO UserQuestionAnswer
                    (
                        AttemptID, AttemptDate, QuestionID, SelectedAnswerID, UserId, IsCorrect,
                        ApprovedBy, CreatedById, CreatedBy, CreatedDate,
                        ModifiedById, ModifiedBy, ModifiedDate,
                        IsActive, Status
                    )
                    VALUES
                    (
                        @AttemptID, @AttemptDate, @QuestionID, @SelectedAnswerID, @UserID, @IsCorrect,
                        @ApprovedBy, @CreatedById, @CreatedBy, @CreatedDate,
                        @ModifiedById, @ModifiedBy, @ModifiedDate,
                        @IsActive, @Status
                    );";

                    foreach (var ans in attempt.UserQuestionAnswers)
                    {
                        ans.UserID = attempt.UserID;
                        ans.IsActive = true;
                      
                        ans.CreatedBy = attempt.CreatedBy;
                        ans.CreatedById = attempt.UserID;
                        ans.CreatedDate = attempt.CreatedDate;
                        ans.AttemptDate = attempt.ModifiedDate;
                        ans.ModifiedDate = attempt.ModifiedDate;
                        ans.ModifiedBy = attempt.ModifiedBy;
                        ans.ModifiedById = attempt.ModifiedById;
                        ans.AttemptID = attemptId;
                        await connection.ExecuteAsync(insertAnswer, ans);
                    }
                }
                return (int)CateoryId;

               
            }
            else // UPDATE
            {
                const string updateAttempt = @"
                UPDATE UserQuizAttempt SET
                    QuizID = @QuizID,
                    AttemptDate = @AttemptDate,
                    Score = @Score,
                    PassScore = @PassScore,
                    Passed = @Passed,
                    ApprovedBy = @ApprovedBy,
                    ModifiedById = @ModifiedById,
                    ModifiedBy = @ModifiedBy,
                    ModifiedDate = @ModifiedDate,
                    IsActive = @IsActive,
                    Status = @Status
                WHERE AttemptID = @AttemptID";
                attempt.AttemptID = AttemptID;
                attempt.AttemptDate = attempt.ModifiedDate;
                await connection.ExecuteAsync(updateAttempt, attempt);
                if (AttemptID > 0)
                {
                    const string updateAnswer = @"
                            UPDATE UserQuestionAnswer SET
                            SelectedAnswerID = @SelectedAnswerID,
                            IsCorrect = @IsCorrect,
                            ApprovedBy = @ApprovedBy,
                            AttemptDate=@AttemptDate,
                            ModifiedById = @ModifiedById,
                            ModifiedBy = @ModifiedBy,
                            ModifiedDate = @ModifiedDate,
                            Status = @Status,
                            QuestionID=@QuestionID
                            WHERE AttemptID = @AttemptID AND UserId = @UserId;
    ";
                    foreach (var ans in attempt.UserQuestionAnswers)
                    {
                        ans.AttemptID = AttemptID;
                        ans.UserID = attempt.UserID;
                        ans.AttemptDate = attempt.ModifiedDate;
                        ans.ModifiedDate = attempt.ModifiedDate;
                        ans.ModifiedBy = attempt.ModifiedBy;
                        ans.ModifiedById = attempt.ModifiedById;
                        await connection.ExecuteAsync(updateAnswer, ans);
                    }
                }
              
                return (int)CateoryId;
            }
        }

        public async Task<int> UpsertUserLessonProgress(UserLessonProgress progress)
        {
            using var connection = _context.CreateConnection();

            var ProgressID = await AlreadyLessonWatch(progress.LessonID, progress.UserID);

            if (ProgressID == 0 || ProgressID == null) // INSERT
            {
                const string insertQuery = @"
        INSERT INTO UserLessonProgress
        (
            UserID, LessonID, Watched, WatchedAt,
            ApprovedBy, CreatedById, CreatedBy, CreatedDate,
            ModifiedById, ModifiedBy, ModifiedDate,
            IsActive, Status
        )
        VALUES
        (
            @UserID, @LessonID, @Watched, @WatchedAt,
            @ApprovedBy, @CreatedById, @CreatedBy, @CreatedDate,
            @ModifiedById, @ModifiedBy, @ModifiedDate,
            @IsActive, @Status
        );
        SELECT CAST(SCOPE_IDENTITY() as int);";

                var newId = await connection.ExecuteScalarAsync<int>(insertQuery, progress);
                return newId;
            }
            else // UPDATE
            {
                const string updateQuery = @"
            UPDATE UserLessonProgress SET
            Watched = @Watched,
            WatchedAt = @WatchedAt,
            ApprovedBy = @ApprovedBy,
            ModifiedById = @ModifiedById,
            ModifiedBy = @ModifiedBy,
            ModifiedDate = @ModifiedDate,
            IsActive = @IsActive,
            Status = @Status
            WHERE ProgressID = @ProgressID";

                progress.ProgressID = ProgressID;
                await connection.ExecuteAsync(updateQuery, progress);
                return progress.ProgressID.Value;
            }
        }

        public async Task<int> AlreadyQuizAttempt(int? quizID, int? userId)
        {
            using var connection = _context.CreateConnection();

            var attemptId = await connection.QueryFirstOrDefaultAsync<int>(
                               "SELECT AttemptID FROM UserQuizAttempt WHERE QuizID = @QuizID AND UserID = @UserID",
                                 new { QuizID = quizID, UserID = userId }
                              );
            return attemptId;
        }

        public async Task<int> AlreadyLessonWatch(int? LessonId, int? userId)
        {
            using var connection = _context.CreateConnection();

            var ProgressID = await connection.QueryFirstOrDefaultAsync<int>(
                               "SELECT ProgressID FROM UserLessonProgress WHERE LessonID = @LessonID AND UserID = @UserID",
                                 new { LessonID = LessonId, UserID = userId }
                              );
            return ProgressID;
        }
    }
}