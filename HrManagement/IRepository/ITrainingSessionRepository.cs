using HrManagement.Dto;

namespace HrManagement.IRepository
{
    public interface ITrainingSessionRepository
    {
        Task<List<DtosTrainingCategoryPost>> WatchLesson(int? id);
    }
}
