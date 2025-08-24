using HrManagement.Dto;
using HrManagement.Models;
using System.Security;

namespace HrManagement.IRepository
{
    public interface IPermissionRepository
    {
        Task<int> UpsertPermissionAsync(TrainingPermission permission);
        Task<DtosTraining> GetTrainingWithPermissions(int userId, DtosTraining training);
        Task<List<TrainingCombinedModel>> TrainingDeshBoard(int userId, List<TrainingCombinedModel> trainings, string UserRoleName);
        Task<TrainingLessonSession> WatchLessonPermissionsUser(int userId, TrainingLessonSession trainings);
        Task<List<TrainingPermissionDto>> GetPermissionRole(string RoleName);


    }

}
