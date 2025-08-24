using HrManagement.Models;

namespace HrManagement.IRepository
{
    public interface IUserQuizRepository
    {
        Task<int> UpsertUserQuizAttempt(UserQuizAttemptModel attempt);

        Task<int> UpsertUserLessonProgress(UserLessonProgress progress);
    }
}