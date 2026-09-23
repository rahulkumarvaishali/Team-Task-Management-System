namespace TaskManagement.API.DTOs.Users
{
    public class UserDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public int? TeamId { get; set; }

        public string? TeamName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
