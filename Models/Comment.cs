namespace Api.Models
{
    public class Comment
    {

        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Content { get; set; }
        public bool IsUpdated { get; set; } = false;
        public int AuthorId { get; set; }
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
        public User Author { get; set; }
        public ICollection<CommentFile> Files { get; set; } = [];
    }
}