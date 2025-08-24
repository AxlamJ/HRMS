using HrManagement.Dto;

namespace HrManagement.IRepository
{
    public interface ITrainingUserProgressReportRepository
    {
        Task<string> GetTrainingUserProgressReport(int trainingId);
    }

}
