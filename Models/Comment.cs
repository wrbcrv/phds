namespace Api.Models
{
    public class Comment
    {

        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Content { get; set; }
        public int AuthorId { get; set; }
        public User Author { get; set; }
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }
}