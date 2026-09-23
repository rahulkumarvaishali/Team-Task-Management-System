using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.API.Interfaces;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService
            _notificationService;

        public NotificationsController(
            INotificationService notificationService)
        {
            _notificationService =
                notificationService;
        }

        private bool TryGetCurrentUserId(
            out int userId)
        {
            var userIdClaim =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            return int.TryParse(
                userIdClaim,
                out userId);
        }

        // GET api/Notifications
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!TryGetCurrentUserId(
                    out var userId))
            {
                return Unauthorized();
            }

            var notifications =
                await _notificationService
                    .GetUserNotificationsAsync(
                        userId);

            return Ok(notifications);
        }

        // PATCH api/Notifications/1/read
        [HttpPatch("{id:int}/read")]
        public async Task<IActionResult> MarkAsRead(
            int id)
        {
            if (!TryGetCurrentUserId(
                    out var userId))
            {
                return Unauthorized();
            }

            var result =
                await _notificationService
                    .MarkAsReadAsync(
                        id,
                        userId);

            if (!result)
            {
                return NotFound(new
                {
                    message =
                        "Notification not found."
                });
            }

            return Ok(new
            {
                message =
                    "Notification marked as read."
            });
        }

        // PATCH api/Notifications/read-all
        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            if (!TryGetCurrentUserId(
                    out var userId))
            {
                return Unauthorized();
            }

            await _notificationService
                .MarkAllAsReadAsync(userId);

            return Ok(new
            {
                message =
                    "All notifications marked as read."
            });
        }
    }
}
