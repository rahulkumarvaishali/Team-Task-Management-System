using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs.Users;
using TaskManagement.API.Interfaces;
using TaskManagement.API.Models.Enums;

namespace TaskManagement.API.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users 
                .AsNoTracking()
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    TeamId = u.TeamId,
                    TeamName = u.Team != null
                        ? u.Team.Name
                        : null,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    TeamId = u.TeamId,
                    TeamName = u.Team != null
                        ? u.Team.Name
                        : null,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateUserRoleAsync(
            int id,
            UserRole role)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return false;
            }

            user.Role = role;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
