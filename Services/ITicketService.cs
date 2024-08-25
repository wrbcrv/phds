using Api.DTOs;
using Api.Models;

namespace Api.Services
{
    public interface ITicketService
    {
        Task<PagedResponseDTO<TicketResponseDTO>> GetAllAsync(int page, int size, TicketFilter filter = null);
        Task<TicketResponseDTO> GetByIdAsync(int id);
        Task<TicketResponseDTO> CreateAsync(TicketDTO ticketDTO);
        Task<TicketResponseDTO> UpdateAsync(int id, TicketDTO ticketDTO);
        Task DeleteAsync(int id);
        Task<CommentResponseDTO> AddCommentAsync(int ticketId, int authorId, string content);
    }
}
