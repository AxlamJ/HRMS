using HrManagement.Models;

namespace HrManagement.IRepository
{
    public interface ITrainingNotifactionRepository
    {
       Task<bool> SendEmail(TrainingAssignModel assignTraining, int maxRetries = 3, int batchSize = 5);
    }
}
