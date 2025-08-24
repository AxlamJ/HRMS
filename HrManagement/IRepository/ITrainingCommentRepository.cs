using HrManagement.Models;

namespace HrManagement.IRepository
{
    public interface ITrainingCommentRepository
    {
        Task<IEnumerable<TrainingComment>> GetCommentsByCategoryAsync(int categoryId, int UserId);
        Task<TrainingComment> GetCommentByIdAsync(int commentId);
        Task<int> AddCommentAsync(TrainingComment comment);
        Task<bool> UpdateCommentAsync(TrainingComment comment);
        Task<bool> DeleteCommentAsync(int commentId);
    }
}
