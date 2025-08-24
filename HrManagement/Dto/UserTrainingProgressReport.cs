using HrManagement.Models;

namespace HrManagement.Dto
{
   
        public class UserTrainingProgressReportTraining
        {
            public int? TrainingID { get; set; }
            public string? TrainingTitle { get; set; }
            public List<UserTrainingProgressTrainingStructure> Structures { get; set; } = new();
        }

        public class UserTrainingProgressTrainingStructure
        {
            public int? TrainingStructureId { get; set; }
            public string? StructureTitle { get; set; }
            public string? StructureType { get; set; }
            public List<UserTrainingProgressTrainingStructureCategory> Categories { get; set; } = new();
        }

        public class UserTrainingProgressTrainingStructureCategory
        {
            public int? TrainingStructureCategoryId { get; set; }
            public string? CategoryTitle { get; set; }
            public UserTrainingProgressLessonProgress? LessonProgress { get; set; }
            public UserTrainingProgressAssessment Assessment { get; set; } = new();
        }

        public class UserTrainingProgressLessonProgress
        {
            public int? ProgressID { get; set; }
            public int? LessonUserID { get; set; }
            public int? LessonID { get; set; }
            public bool? Watched { get; set; }
            public DateTime? WatchedAt { get; set; }
        }

        public class UserTrainingProgressAssessment
        {
            public int? AssessmentID { get; set; }
            public string? QuizTitle { get; set; }
            public int? QuizPassScore { get; set; }
            public List<UserTrainingProgressAssessmentQuestion> Questions { get; set; } = new();
            public List<UserTrainingProgressUserQuizAttempt> Attempts { get; set; } = new();
        }

        public class UserTrainingProgressAssessmentQuestion
        {
            public int? AssessmentQuestionId { get; set; }
            public List<UserTrainingProgressUserAssessmentAnswer> Answers { get; set; } = new();
            public List<UserTrainingProgressUserQuestionAnswer> UserAnswers { get; set; } = new();
        }

        public class UserTrainingProgressUserAssessmentAnswer
        {
            public int? AssessmentAnswerId { get; set; }
            public bool? AnswerIsCorrect { get; set; }
        }

        public class UserTrainingProgressUserQuizAttempt
        {
            public int? AttemptID { get; set; }
            public int? QuizUserID { get; set; }
            public int? QuizScore { get; set; }
            public bool? QuizPassed { get; set; }
            public int? AttemptPassScore { get; set; }
            public DateTime? AttemptDate { get; set; }
        }

        public class UserTrainingProgressUserQuestionAnswer
        {
            public int? UserAnswerID { get; set; }
            public int? SelectedAnswerID { get; set; }
            public bool? UserAnswerIsCorrect { get; set; }
        }
    }



