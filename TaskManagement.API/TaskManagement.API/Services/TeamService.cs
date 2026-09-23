using TaskManagement.API.Data;
using TaskManagement.API.DTOs.Teams;
using TaskManagement.API.Models.Enums;
using TaskManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Interfaces;

namespace TaskManagement.API.Services
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _context;

        public TeamService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL TEAMS
        public async Task<IEnumerable<TeamDto>> GetAllTeamsAsync()
        {
            return await _context.Teams
                .AsNoTracking()
                .Select(t => new TeamDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    ManagerId = t.ManagerId,
                    ManagerName = t.Manager != null
                        ? t.Manager.FullName
                        : null,
                    MemberCount = t.Members.Count,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        // GET TEAM BY ID
        public async Task<TeamDto?> GetTeamByIdAsync(int id)
        {
            return await _context.Teams
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new TeamDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    ManagerId = t.ManagerId,
                    ManagerName = t.Manager != null
                        ? t.Manager.FullName
                        : null,
                    MemberCount = t.Members.Count,
                    CreatedAt = t.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        // CREATE TEAM
        public async Task<TeamDto?> CreateTeamAsync(
            CreateTeamDto dto)
        {
            if (dto.ManagerId.HasValue)
            {
                var manager = await _context.Users
                    .FirstOrDefaultAsync(u =>
                        u.Id == dto.ManagerId.Value);

                if (manager == null ||
                    manager.Role != UserRole.Manager)
                {
                    return null;
                }
            }

            var team = new Team
            {
                Name = dto.Name.Trim(),
                Description = dto.Description,
                ManagerId = dto.ManagerId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Teams.Add(team);

            await _context.SaveChangesAsync();

            return await GetTeamByIdAsync(team.Id);
        }

        // UPDATE TEAM
        public async Task<bool> UpdateTeamAsync(
            int id,
            UpdateTeamDto dto)
        {
            var team = await _context.Teams
                .FindAsync(id);

            if (team == null)
            {
                return false;
            }

            if (dto.ManagerId.HasValue)
            {
                var manager = await _context.Users
                    .FirstOrDefaultAsync(u =>
                        u.Id == dto.ManagerId.Value);

                if (manager == null ||
                    manager.Role != UserRole.Manager)
                {
                    return false;
                }
            }

            team.Name = dto.Name.Trim();
            team.Description = dto.Description;
            team.ManagerId = dto.ManagerId;

            await _context.SaveChangesAsync();

            return true;
        }

        // DELETE TEAM
        public async Task<bool> DeleteTeamAsync(int id)
        {
            var team = await _context.Teams
                .FindAsync(id);

            if (team == null)
            {
                return false;
            }

            _context.Teams.Remove(team);

            await _context.SaveChangesAsync();

            return true;
        }

        // ADD MEMBER TO TEAM
        public async Task<bool> AddMemberAsync(
            int teamId,
            int userId,
            int currentUserId,
            string currentUserRole)
        {
            var team = await _context.Teams
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
            {
                return false;
            }

            // Manager can manage only their own team
            if (currentUserRole == "Manager" &&
                team.ManagerId != currentUserId)
            {
                return false;
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return false;
            }

            user.TeamId = teamId;

            await _context.SaveChangesAsync();

            return true;
        }

        // REMOVE MEMBER FROM TEAM
        public async Task<bool> RemoveMemberAsync(
            int teamId,
            int userId,
            int currentUserId,
            string currentUserRole)
        {
            var team = await _context.Teams
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
            {
                return false;
            }

            // Manager can manage only their own team
            if (currentUserRole == "Manager" &&
                team.ManagerId != currentUserId)
            {
                return false;
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Id == userId &&
                    u.TeamId == teamId);

            if (user == null)
            {
                return false;
            }

            user.TeamId = null;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
