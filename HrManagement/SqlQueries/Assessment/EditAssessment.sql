
SELECT -- Assessment
 a.Id AS AssessmentId,
 a.Title AS AssessmentTitle,
 a.PassingGrade AS AssessmentPassingGrade,
 a.IsPassingGrade AS AssessmentIsPassingGrade,
 a.UserId AS AssessmentUserId,
 a.CategoryId AS AssessmentCategoryId,
 a.IsActive AS AssessmentIsActive,
 a.Status AS AssessmentStatus,
 a.PassConfirmationMessage AS AssessmentPassMessage,
 a.FailConfirmationMessage AS AssessmentFailMessage,
 -- AssessmentQuestion
 q.Id AS QuestionId,
 q.Title AS QuestionTitle,
 q.UserId AS QuestionUserId,
 q.CategoryId AS QuestionCategoryId,
 q.IsActive AS QuestionIsActive,
 q.Type AS QuestionType,
 -- AssessmentAnswer
 ans.Id AS AnswerId,
 ans.Title AS AnswerTitle,
 ans.IsCorrect,
 ans.UserId AS AnswerUserId,
 ans.CategoryId AS AnswerCategoryId,
 ans.AssessmentQuestionId,
 ans.IsActive AS AnswerIsActive,
 ans.Type AS AnswerType,
 -- [FileMediaUplaod]
 mf.Id AS FileId,
 mf.FileName,
 mf.FilePath,
 mf.ContentType,
 mf.Size,
 mf.ModuleName,
 mf.ReferenceId,
 mf.Extension,
 mf.IsActive AS FileIsActive,
 mf.Type AS FileType
FROM Assessment a
LEFT JOIN AssessmentQuestion q ON q.CategoryId = a.CategoryId
LEFT JOIN AssessmentAnswer ans ON ans.AssessmentQuestionId = q.Id
LEFT JOIN FileMediaUplaod mf ON mf.ReferenceId = a.CategoryId AND mf.ModuleName = @ModuleName AND mf.IsActive = 1
WHERE a.CategoryId = @Id;