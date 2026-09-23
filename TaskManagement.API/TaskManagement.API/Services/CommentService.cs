using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs.Comments;
using TaskManagement.API.Interfaces;
using TaskManagement.API.Models;

namespace TaskManagement.API.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<bool> CanAccessTaskAsync(
            int taskId,
            int currentUserId,
            string currentUserRole)
        {
            var task = await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
                return false;

            // Admin can access every task
            if (currentUserRole == "Admin")
                return true;

            // User can access only assigned tasks
            if (currentUserRole == "User")
            {
                return task.AssignedToUserId ==
                       currentUserId;
            }

            // Manager can access tasks belonging
            // to a team managed by them
            if (currentUserRole == "Manager")
            {
                if (!task.TeamId.HasValue)
                    return false;

                return await _context.Teams
                    .AnyAsync(t =>
                        t.Id == task.TeamId.Value &&
                        t.ManagerId == currentUserId);
            }

            return false;
        }

        public async Task<IEnumerable<CommentDto>?>
            GetCommentsAsync(
                int taskId,
                int currentUserId,
                string currentUserRole)
        {
            var canAccess =
                await CanAccessTaskAsync(
                    taskId,
                    currentUserId,
                    currentUserRole);

            if (!canAccess)
                return null;

            return await _context.Comments
                .AsNoTracking()
                .Where(c => c.TaskItemId == taskId)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    TaskItemId = c.TaskItemId,
                    UserId = c.UserId,
                    UserName = c.User.FullName,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<CommentDto?> AddCommentAsync(
            int taskId,
            CreateCommentDto dto,
            int currentUserId,
            string currentUserRole)
        {
            var canAccess =
                await CanAccessTaskAsync(
                    taskId,
                    currentUserId,
                    currentUserRole);

            if (!canAccess)
                return null;

            var comment = new Comment
            {
                Content = dto.Content.Trim(),
                TaskItemId = taskId,
                UserId = currentUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);

            await _context.SaveChangesAsync();

            return await _context.Comments
                .AsNoTracking()
                .Where(c => c.Id == comment.Id)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    TaskItemId = c.TaskItemId,
                    UserId = c.UserId,
                    UserName = c.User.FullName,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteCommentAsync(
            int commentId,
            int currentUserId,
            string currentUserRole)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c =>
                    c.Id == commentId);

            if (comment == null)
                return false;

            // Admin can delete any comment.
            // Other users can delete only their own.
            if (currentUserRole != "Admin" &&
                comment.UserId != currentUserId)
            {
                return false;
            }

            _context.Comments.Remove(comment);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
