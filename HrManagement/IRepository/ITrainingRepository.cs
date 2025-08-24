using HrManagement.Dto;
using HrManagement.Models;
using System.Data;

namespace HrManagement.IRepository
{
    public interface ITrainingRepository
    {
        Task<bool> UpsertTrainingStructure(TrainingStructure  training);
        Task<bool> UpsertTrainingDetail(TrainingsDetail trainingsDetail);
        Task<(List<TrainingCombinedModel> Data, int TotalCount)> GetTrainingsCombinedAsync(int UserId, string UserRoleName);
        Task<(List<TrainingCombinedModel> Data, int TotalCount)> GetTrainingsAdmin();
        Task<int> AddUpdateTrainingPostDetails(TrainingPostDetails model);
        Task<int> UpsertQuestion(AssessmentQuestion question);
        Task UpsertAnswer(AssessmentAnswer answer);
        Task<int> AddOrUpdateAssessment(Assessment model);
        Task<List<AssessmentDto>> GetAssessmentsFromFileAsync(int categoryId);
        Task<int> AddUpdateTrainingOutline(TrainingOutline model);
        Task<int> CloneFullTrainingAsync(int trainingId);

    }
}
