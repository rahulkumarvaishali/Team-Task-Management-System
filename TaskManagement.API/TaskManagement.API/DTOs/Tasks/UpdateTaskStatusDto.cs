using TaskStatusEnum = TaskManagement.API.Models.Enums.TaskStatus;

namespace TaskManagement.API.DTOs.Tasks
{
    public class UpdateTaskStatusDto
    {
        public TaskStatusEnum Status { get; set; }

    }
}
