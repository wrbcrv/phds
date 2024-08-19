using Api.Models;

namespace Api.DTOs
{
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public AgencyHierarchyDTO Agency { get; set; }
        public List<AssignedTicketResponseDTO> AssignedTickets { get; set; } = new List<AssignedTicketResponseDTO>();

        public static UserResponseDTO ValueOf(User user)
        {
            return new UserResponseDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Role = user.Role.ToString(),
                Agency = user.Agency != null ? AgencyHierarchyDTO.ValueOf(user.Agency) : null,
                AssignedTickets = user.AssignedTickets?.Select(AssignedTicketResponseDTO.FromTicket).ToList()
            };
        }
    }
}
