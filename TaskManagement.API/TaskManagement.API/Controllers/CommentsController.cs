using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.API.DTOs.Comments;
using TaskManagement.API.Interfaces;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(
            ICommentService commentService)
        {
            _commentService = commentService;
        }

        // GET: /{taskId}/comments
        [HttpGet("{taskId:int}/comments")]
        public async Task<IActionResult> GetComments(
            int taskId)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            var comments =
                await _commentService.GetCommentsAsync(
                    taskId,
                    userId,
                    role);

            return Ok(comments);
        }

        // POST: /{taskId}/comments
        [HttpPost("{taskId:int}/comments")]
        public async Task<IActionResult> AddComment(
            int taskId,
            [FromBody] CreateCommentDto dto)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            var comment =
                await _commentService.AddCommentAsync(
                    taskId,
                    dto,
                    userId,
                    role);

            if (comment == null)
            {
                return BadRequest(new
                {
                    message =
                        "Unable to add comment. Check task access."
                });
            }

            return Ok(comment);
        }

        // DELETE: /api/comments/{commentId}
        [HttpDelete("api/comments/{commentId:int}")]
        public async Task<IActionResult> DeleteComment(
            int commentId)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            var deleted =
                await _commentService.DeleteCommentAsync(
                    commentId,
                    userId,
                    role);

            if (!deleted)
            {
                return NotFound(new
                {
                    message =
                        "Comment not found or you do not have permission."
                });
            }

            return Ok(new
            {
                message =
                    "Comment deleted successfully."
            });
        }

        private int GetCurrentUserId()
        {
            var value =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!int.TryParse(
                    value,
                    out var userId))
            {
                throw new UnauthorizedAccessException(
                    "Invalid user token.");
            }

            return userId;
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirstValue(
                       ClaimTypes.Role)
                   ?? string.Empty;
        }
    }
}
