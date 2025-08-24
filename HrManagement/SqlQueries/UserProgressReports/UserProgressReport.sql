DECLARE @jsonResult NVARCHAR(MAX);

SET @jsonResult = (
    SELECT
        t.TrainingID,
        t.Title AS TrainingTitle,
        (
            SELECT
                ts.TrainingStructureId,
                ts.Title AS StructureTitle,
                ts.Type AS StructureType,
                (
                    SELECT
                        tsc.Id AS TrainingStructureCategoryId,
                        tsc.Title AS CategoryTitle,
                        tsc.Type AS CategoryType,
                        (
                            SELECT
                                ulp.ProgressID,
                                ulp.UserID AS LessonUserID,
                                ulp.LessonID,
                                ulp.Watched,
                                ulp.WatchedAt
                            FROM UserLessonProgress ulp
                            WHERE ulp.LessonID = tsc.Id AND tsc.Type<>'5'
                            FOR JSON PATH -- returns JSON array here, no WITHOUT_ARRAY_WRAPPER
                        ) AS LessonProgress,
                        JSON_QUERY( -- embed assessment JSON properly
                            (
                                SELECT
                                    a.Id AS AssessmentID,
                                    a.Title AS QuizTitle,
                                    a.PassingGrade AS QuizPassScore,
                                    (
                                        SELECT
                                            aq.Id AS AssessmentQuestionId,
                                            (
                                                SELECT
                                                    aa.Id AS AssessmentAnswerId,
                                                    aa.IsCorrect AS AnswerIsCorrect
                                                FROM AssessmentAnswer aa
                                                WHERE aa.AssessmentQuestionID = aq.Id
                                                FOR JSON PATH
                                            ) AS Answers,
                                            (
                                                SELECT
                                                    uqan.UserAnswerID,
                                                    uqan.SelectedAnswerID,
                                                    uqan.IsCorrect AS UserAnswerIsCorrect
                                                FROM UserQuestionAnswer uqan
                                                WHERE uqan.QuestionID = aq.Id
                                                FOR JSON PATH
                                            ) AS UserAnswers
                                        FROM AssessmentQuestion aq
                                        WHERE aq.CategoryId = a.Id
                                        FOR JSON PATH
                                    ) AS Questions,
                                    (
                                        SELECT
                                            uqa.AttemptID,
                                            uqa.UserID AS QuizUserID,
                                            uqa.Score AS QuizScore,
                                            uqa.Passed AS QuizPassed,
                                            uqa.PassScore AS AttemptPassScore,
                                            uqa.AttemptDate
                                        FROM UserQuizAttempt uqa
                                        WHERE uqa.QuizID = a.Id
                                        FOR JSON PATH
                                    ) AS Attempts
                                FROM Assessment a
                                WHERE a.CategoryId = tsc.Id
                                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                            )
                        ) AS Assessment
                    FROM TrainingStructureCategory tsc
                    WHERE tsc.TrainingStructureId = ts.TrainingStructureId AND tsc.Status='1'
                    FOR JSON PATH
                ) AS Categories
            FROM TrainingStructure ts
            WHERE ts.TrainingId = t.TrainingID AND ts.Status='1'
            FOR JSON PATH
        ) AS Structures
    FROM Trainings t
    WHERE t.IsActive = 1 AND t.TrainingId = @TrainingId
    FOR JSON PATH, ROOT('Trainings')
);

SELECT @jsonResult AS JsonResult;
