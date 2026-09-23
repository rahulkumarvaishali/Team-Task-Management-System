using TaskManagement.API.DTOs.Tasks;

namespace TaskManagement.API.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetTasksAsync(
    int currentUserId,
    string currentUserRole,
    int? status = null,
    int? priority = null,
    DateTime? deadline = null);

        Task<TaskDto?> GetTaskByIdAsync(
            int taskId,
            int currentUserId,
            string currentUserRole);

        Task<TaskDto?> CreateTaskAsync(
            CreateTaskDto dto,
            int currentUserId,
            string currentUserRole);

        Task<bool> UpdateTaskAsync(
            int taskId,
            UpdateTaskDto dto,
            int currentUserId,
            string currentUserRole);

        Task<bool> UpdateTaskStatusAsync(
            int taskId,
            UpdateTaskStatusDto dto,
            int currentUserId,
            string currentUserRole);

        Task<bool> DeleteTaskAsync(
            int taskId,
            int currentUserId,
            string currentUserRole);

        Task<DashboardDto> GetDashboardAsync(
            int currentUserId,
            string currentUserRole);

        Task<IEnumerable<TaskActivityDto>> GetTaskActivityAsync(
    int taskId);
    }
}
