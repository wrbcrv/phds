namespace Api.Models
{
    public class Agency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public virtual Agency Parent { get; set; }
        public virtual ICollection<Agency> Children { get; set; } = new HashSet<Agency>();
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public ICollection<User> Users { get; set; }
    }
}
