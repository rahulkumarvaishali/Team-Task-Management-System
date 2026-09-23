namespace TaskManagement.API.Models
{
    public class Team
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? ManagerId { get; set; }

        public User? Manager { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<User> Members { get; set; }
            = new List<User>();

        public ICollection<TaskItem> Tasks { get; set; }
            = new List<TaskItem>();
    }
}
