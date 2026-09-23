using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.API.DTOs.Teams;
using TaskManagement.API.Interfaces;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAll()
        {
            var teams =
                await _teamService.GetAllTeamsAsync();

            return Ok(teams);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetById(int id)
        {
            var team =
                await _teamService.GetTeamByIdAsync(id);

            if (team == null)
            {
                return NotFound(new
                {
                    message = "Team not found."
                });
            }

            return Ok(team);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            CreateTeamDto dto)
        {
            var team =
                await _teamService.CreateTeamAsync(dto);

            if (team == null)
            {
                return BadRequest(new
                {
                    message =
                        "Invalid Manager. Manager must exist and have Manager role."
                });
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = team.Id },
                team);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            int id,
            UpdateTeamDto dto)
        {
            var result =
                await _teamService.UpdateTeamAsync(id, dto);

            if (!result)
            {
                return BadRequest(new
                {
                    message =
                        "Team not found or Manager is invalid."
                });
            }

            return Ok(new
            {
                message = "Team updated successfully."
            });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result =
                await _teamService.DeleteTeamAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Team not found."
                });
            }

            return Ok(new
            {
                message = "Team deleted successfully."
            });
        }

        [HttpPost("{teamId:int}/members/{userId:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddMember(
    int teamId,
    int userId)
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            var role =
                User.FindFirstValue(ClaimTypes.Role);

            if (!int.TryParse(userIdClaim, out var currentUserId) ||
                string.IsNullOrEmpty(role))
            {
                return Unauthorized();
            }

            var result = await _teamService.AddMemberAsync(
                teamId,
                userId,
                currentUserId,
                role);

            if (!result)
            {
                return BadRequest(new
                {
                    message =
                        "Unable to add user. Check team, user, or permissions."
                });
            }

            return Ok(new
            {
                message = "User added to team successfully."
            });
        }

        [HttpDelete("{teamId:int}/members/{userId:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RemoveMember(
    int teamId,
    int userId)
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            var role =
                User.FindFirstValue(ClaimTypes.Role);

            if (!int.TryParse(userIdClaim, out var currentUserId) ||
                string.IsNullOrEmpty(role))
            {
                return Unauthorized();
            }

            var result = await _teamService.RemoveMemberAsync(
                teamId,
                userId,
                currentUserId,
                role);

            if (!result)
            {
                return BadRequest(new
                {
                    message =
                        "Unable to remove user. Check team, user, or permissions."
                });
            }

            return Ok(new
            {
                message =
                    "User removed from team successfully."
            });
        }
    }
}
