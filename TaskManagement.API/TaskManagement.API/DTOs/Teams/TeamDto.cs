namespace TaskManagement.API.DTOs.Teams
{
    public class TeamDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? ManagerId { get; set; }

        public string? ManagerName { get; set; }

        public int MemberCount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
