namespace Api.Models
{
    public class Agency
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ParentId { get; set; }
        public virtual Agency? Parent { get; set; }
        public virtual ICollection<Agency> Children { get; set; } = [];
        public ICollection<Ticket> Tickets { get; set; } = [];
        public ICollection<User>? Users { get; set; }
        public bool IsTopLevel => ParentId == null;
    }
}
