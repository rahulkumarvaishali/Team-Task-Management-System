using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs.Teams
{
    public class CreateTeamDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public int? ManagerId { get; set; }
    }
}
