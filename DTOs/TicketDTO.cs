using Api.Models;
using Type = Api.Models.Type;

namespace Api.DTOs
{
    public class TicketDTO
    {
        public string Subject { get; set; }
        public string Description { get; set; }
        public Type Type { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public int LocationId { get; set; }
        public Category Category { get; set; }
        public int[] CustomerIds { get; set; } = new int[0];
        public int[] AssigneeIds { get; set; } = new int[0];
    }
}
