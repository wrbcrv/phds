using Api.Models;

namespace Api.DTOs
{
    public class CommentResponseDTO
    {
        public int Id { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public bool IsUpdated { get; set; }
        public string Content { get; set; }
        public UserSummaryResponseDTO Author { get; set; }
        public bool CanDelete { get; set; }
        public List<FileDTO> Files { get; set; }

        public static CommentResponseDTO ValueOf(Comment comment)
        {
            TimeSpan removalTimeLimit = TimeSpan.FromMinutes(5);
            var timeSinceCommentCreated = DateTime.UtcNow - comment.CreatedAt;

            return new CommentResponseDTO
            {
                Id = comment.Id,
                CreatedAt = comment.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                UpdatedAt = comment.UpdatedAt.ToString("dd/MM/yyyy HH:mm"),
                IsUpdated = comment.IsUpdated,
                Content = comment.Content,
                Author = comment.Author != null ? UserSummaryResponseDTO.ValueOf(comment.Author) : null,
                CanDelete = timeSinceCommentCreated <= removalTimeLimit,
                Files = comment.Files?.Select(f => new FileDTO
                {
                    FilePath = f.FilePath,
                    FileName = f.FileName
                }).ToList()
            };
        }
    }
}
