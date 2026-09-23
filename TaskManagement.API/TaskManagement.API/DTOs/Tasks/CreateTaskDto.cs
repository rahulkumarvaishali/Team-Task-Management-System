using System.ComponentModel.DataAnnotations;
using TaskManagement.API.Models.Enums;

namespace TaskManagement.API.DTOs.Tasks
{
    public class CreateTaskDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public TaskPriority Priority { get; set; }
            = TaskPriority.Medium;

        public DateTime? Deadline { get; set; }

        public int? AssignedToUserId { get; set; }

        public int? TeamId { get; set; }
    }
}
