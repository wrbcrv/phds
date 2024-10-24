using Api.DTOs;
using Api.Models;

namespace Api.Services.Interfaces
{
    public interface ITicketService
    {
        Task<PagedResponseDTO<TicketResponseDTO>> GetAllAsync(int page, int size, TicketFilter filter = null);
        Task<TicketResponseDTO> GetByIdAsync(int id);
        Task<TicketResponseDTO> CreateAsync(TicketDTO ticketDTO);
        Task<TicketResponseDTO> UpdateAsync(int id, TicketDTO ticketDTO);
        Task DeleteAsync(int id);
        Task<TicketResponseDTO> AssignCurrentUserAsync(int ticketId, int userId, bool asAssignee);
        Task<TicketResponseDTO> AssignEntitiesAsync(int ticketId, List<int> entityIds, string entityType);
        Task<TicketResponseDTO> RemoveEntityAsync(int ticketId, int entityId, string entityType);
    }
}
