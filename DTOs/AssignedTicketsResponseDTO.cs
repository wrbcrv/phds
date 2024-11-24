using Api.Models;

namespace Api.DTOs
{
    public class AssignedTicketResponseDTO
    {
        public int Id { get; set; }
        public string? Subject { get; set; }

        public static AssignedTicketResponseDTO FromTicket(Ticket ticket)
        {
            return new AssignedTicketResponseDTO
            {
                Id = ticket.Id,
                Subject = ticket.Subject
            };
        }
    }
}
