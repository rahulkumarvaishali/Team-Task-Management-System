using TaskManagement.API.Models.Enums;

namespace TaskManagement.API.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.User;

        public int? TeamId { get; set; }

        public Team? Team { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<TaskItem> AssignedTasks { get; set; }
            = new List<TaskItem>();

        public ICollection<TaskItem> CreatedTasks { get; set; }
            = new List<TaskItem>();

        public ICollection<Comment> Comments { get; set; }
            = new List<Comment>();

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}
