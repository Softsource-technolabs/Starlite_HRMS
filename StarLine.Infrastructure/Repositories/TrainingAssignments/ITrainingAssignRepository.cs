using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.TrainingAssignments
{
    public interface ITrainingAssignRepository
    {
        Task<List<TrainingSessionModel>> GetAllSessionsAsync();
        Task<List<EmployeeModel>> GetEligibleEmployeesAsync(long trainingSessionId);
        Task<BaseApiResponse> AssignEmployeesAsync(long sessionId, List<long> employeeIds);
        Task<List<TrainingAssignmentModel>> GetAssignmentsBySessionAsync(long sessionId);
        Task<List<TrainingAssignmentListModel>> GetAllAssignmentsAsync();
        Task<List<TrainingAssignmentListModel>> GetSessionAssignedEmployeeList(long sessionId);
        Task<List<TrainerSessionListModel>> GetAllSessionsByTrainerAsync(long trainerId);
        Task<bool> UpdateCompletionStatusAsync(List<TrainingSessionCompletionModel> Session);
        Task GenerateCertificate(long certificateId);
    }
}
