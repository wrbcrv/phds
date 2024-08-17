namespace Api.Models
{

    public class Ticket
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Subject { get; set; }
        public string Description { get; set; }
        public Type Type { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public int? AgencyId { get; set; }
        public Agency Location { get; set; }
        public ICollection<User> Customers { get; set; } = new List<User>();
        public ICollection<User> Assignees { get; set; } = new List<User>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}