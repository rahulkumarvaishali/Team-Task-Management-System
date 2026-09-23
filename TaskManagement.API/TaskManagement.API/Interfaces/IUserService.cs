using TaskManagement.API.DTOs.Users;
using TaskManagement.API.Models.Enums;

namespace TaskManagement.API.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<UserDto?> GetUserByIdAsync(int id);

        Task<bool> UpdateUserRoleAsync(int id, UserRole role);
    }
}
