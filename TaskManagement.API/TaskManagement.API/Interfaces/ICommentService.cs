using TaskManagement.API.DTOs.Comments;

namespace TaskManagement.API.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>?> GetCommentsAsync(
            int taskId,
            int currentUserId,
            string currentUserRole);

        Task<CommentDto?> AddCommentAsync(
            int taskId,
            CreateCommentDto dto,
            int currentUserId,
            string currentUserRole);

        Task<bool> DeleteCommentAsync(
            int commentId,
            int currentUserId,
            string currentUserRole);
    }
}
