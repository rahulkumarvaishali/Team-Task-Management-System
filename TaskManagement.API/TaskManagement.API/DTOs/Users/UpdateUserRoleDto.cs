using System.ComponentModel.DataAnnotations;
using TaskManagement.API.Models.Enums;

namespace TaskManagement.API.DTOs.Users
{
    public class UpdateUserRoleDto
    {
        [Required]
        public UserRole Role { get; set; }
    }
}
