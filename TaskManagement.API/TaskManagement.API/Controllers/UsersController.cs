using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs.Users;
using TaskManagement.API.Interfaces;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // Admin and Manager can view users
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users =
                await _userService.GetAllUsersAsync();

            return Ok(users);
        }


        // Admin and Manager can view a user
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user =
                await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            return Ok(user);
        }


        // Only Admin can change roles
        [HttpPatch("{id:int}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRole(            int id,
            UpdateUserRoleDto dto)
        {
            var result =
                await _userService.UpdateUserRoleAsync(
                    id,
                    dto.Role
                );

            if (!result)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            return Ok(new
            {
                message = "User role updated successfully."
            });
        }
    }
}
