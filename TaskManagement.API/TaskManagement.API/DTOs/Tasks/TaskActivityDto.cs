namespace TaskManagement.API.DTOs.Tasks
{
    public class TaskActivityDto
    {
        public int Id { get; set; }

        public string PerformedByName { get; set; }
            = string.Empty;

        public string PerformedByRole { get; set; }
            = string.Empty;

        public string Action { get; set; }
            = string.Empty;

        public string Details { get; set; }
            = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
