namespace Api.Models
{
    public class TicketFilter
    {
        public Status? Status { get; set; }
        public Priority? Priority { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public string? Subject { get; set; }
        public string? CustomerName { get; set; }
        public int? LocationId { get; set; }
    }
}