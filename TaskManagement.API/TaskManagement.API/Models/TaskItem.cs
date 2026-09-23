using TaskManagement.API.Models.Enums;
using TaskStatusEnum = TaskManagement.API.Models.Enums.TaskStatus;
namespace TaskManagement.API.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TaskStatusEnum Status { get; set; } = TaskStatusEnum.ToDo;

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public DateTime? Deadline { get; set; }

        public int? AssignedToUserId { get; set; }

        public User? AssignedToUser { get; set; }

        public int CreatedByUserId { get; set; }

        public User CreatedByUser { get; set; } = null!;

        public int? TeamId { get; set; }

        public Team? Team { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Comment> Comments { get; set; }
            = new List<Comment>();
    }
}
