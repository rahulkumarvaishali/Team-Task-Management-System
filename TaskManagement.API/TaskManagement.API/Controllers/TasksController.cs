using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.API.DTOs.Tasks;
using TaskManagement.API.Interfaces;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // Get logged-in user information from JWT
        private bool TryGetCurrentUser(
            out int currentUserId,
            out string currentUserRole)
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            currentUserRole =
                User.FindFirstValue(ClaimTypes.Role)
                ?? string.Empty;

            return int.TryParse(
                       userIdClaim,
                       out currentUserId)
                   &&
                   !string.IsNullOrWhiteSpace(
                       currentUserRole);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
    [FromQuery] int? status,
    [FromQuery] int? priority,
    [FromQuery] DateTime? deadline)
        {
            if (!TryGetCurrentUser(
                    out var userId,
                    out var role))
            {
                return Unauthorized();
            }

            var tasks = await _taskService.GetTasksAsync(
                userId,
                role,
                status,
                priority,
                deadline);

            return Ok(tasks);
        }

        // GET: api/Tasks/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!TryGetCurrentUser(
                    out var userId,
                    out var role))
            {
                return Unauthorized();
            }

            var task =
                await _taskService.GetTaskByIdAsync(
                    id,
                    userId,
                    role);

            if (task == null)
            {
                return NotFound(new
                {
                    message =
                        "Task not found or access denied."
                });
            }

            return Ok(task);
        }

        // POST: api/Tasks
        // Only Admin and Manager can create tasks
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(
            CreateTaskDto dto)
        {
            if (!TryGetCurrentUser(
                    out var userId,
                    out var role))
            {
                return Unauthorized();
            }

            var task =
                await _taskService.CreateTaskAsync(
                    dto,
                    userId,
                    role);

            if (task == null)
            {
                return BadRequest(new
                {
                    message =
                        "Unable to create task. Check team, assigned user, and permissions."
                });
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = task.Id },
                task);
        }

        // PUT: api/Tasks/1
        // User cannot edit complete task details
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(
            int id,
            UpdateTaskDto dto)
        {
            if (!TryGetCurrentUser(
                    out var userId,
                    out var role))
            {
                return Unauthorized();
            }

            var result =
                await _taskService.UpdateTaskAsync(
                    id,
                    dto,
                    userId,
                    role);

            if (!result)
            {
                return BadRequest(new
                {
                    message =
                        "Unable to update task. Check task, team, user, and permissions."
                });
            }

            return Ok(new
            {
                message =
                    "Task updated successfully."
            });
        }

        // PATCH: api/Tasks/1/status
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            UpdateTaskStatusDto dto)
        {
            if (!TryGetCurrentUser(
                    out var userId,
                    out var role))
            {
                return Unauthorized();
            }

            var result =
                await _taskService.UpdateTaskStatusAsync(
                    id,
                    dto,
                    userId,
                    role);

            if (!result)
            {
                return BadRequest(new
                {
                    message =
                        "Unable to update status. Task not found or access denied."
                });
            }

            return Ok(new
            {
                message =
                    "Task status updated successfully."
            });
        }

        // DELETE: api/Tasks/1
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!TryGetCurrentUser(
                    out var userId,
                    out var role))
            {
                return Unauthorized();
            }

            var result =
                await _taskService.DeleteTaskAsync(
                    id,
                    userId,
                    role);

            if (!result)
            {
                return BadRequest(new
                {
                    message =
                        "Unable to delete task. Task not found or access denied."
                });
            }

            return Ok(new
            {
                message =
                    "Task deleted successfully."
            });
        }

        // GET: api/Tasks/1/activity
        // Activity history is visible only to Admin
        [HttpGet("{id:int}/activity")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActivity(int id)
        {
            if (!TryGetCurrentUser(
                    out var userId,
                    out var role))
            {
                return Unauthorized();
            }

            // Make sure the task exists and Admin
            // has access to the task.
            var task =
                await _taskService.GetTaskByIdAsync(
                    id,
                    userId,
                    role);

            if (task == null)
            {
                return NotFound(new
                {
                    message = "Task not found."
                });
            }

            var activities =
                await _taskService.GetTaskActivityAsync(id);

            return Ok(activities);
        }

        // GET api/Tasks/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            if (!TryGetCurrentUser(
                    out var userId,
                    out var role))
            {
                return Unauthorized();
            }

            var dashboard =
                await _taskService.GetDashboardAsync(
                    userId,
                    role);

            return Ok(dashboard);
        }
    }
}
