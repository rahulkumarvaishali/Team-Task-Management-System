namespace TaskManagement.API.DTOs.Comments
{
    public class CommentDto
    {
        public int Id { get; set; }

        public string Content { get; set; } = string.Empty;

        public int TaskItemId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
