using TaskManagement.API.DTOs.Teams;

namespace TaskManagement.API.Interfaces
{
    public interface ITeamService
    {
        // Get all teams
        Task<IEnumerable<TeamDto>> GetAllTeamsAsync();

        // Get team by ID
        Task<TeamDto?> GetTeamByIdAsync(int id);

        // Create new team
        Task<TeamDto?> CreateTeamAsync(CreateTeamDto dto);

        // Update existing team
        Task<bool> UpdateTeamAsync(
            int id,
            UpdateTeamDto dto);

        // Delete team
        Task<bool> DeleteTeamAsync(int id);

        // Add user to team
        // currentUserId and currentUserRole are used
        // to check Manager ownership
        Task<bool> AddMemberAsync(
            int teamId,
            int userId,
            int currentUserId,
            string currentUserRole);

        // Remove user from team
        Task<bool> RemoveMemberAsync(
            int teamId,
            int userId,
            int currentUserId,
            string currentUserRole);
    }
}
