namespace Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Role Role { get; set; }
        public int? AgencyId { get; set; }
        public Agency? Agency { get; set; }
        public ICollection<Ticket> CreatedTickets { get; set; } = [];
        public ICollection<Ticket> AssignedTickets { get; set; } = [];
        public ICollection<Ticket> ObservedTickets { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
        public ICollection<Notification> Notifications { get; set; } = [];
    }
}