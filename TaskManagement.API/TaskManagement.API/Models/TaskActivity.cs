namespace TaskManagement.API.Models
{
    public class TaskActivity
    {
        public int Id { get; set; }

        public int TaskId { get; set; }

        public int PerformedByUserId { get; set; }

        public string PerformedByName { get; set; }
            = string.Empty;

        public string PerformedByRole { get; set; }
            = string.Empty;

        public string Action { get; set; }
            = string.Empty;

        public string Details { get; set; }
            = string.Empty;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public TaskItem Task { get; set; }
            = null!;
    }
}
