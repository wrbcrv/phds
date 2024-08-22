using Api.DTOs;

namespace Api.Services
{
    public interface ITicketService
    {
        Task<PagedResponseDTO<TicketResponseDTO>> GetAllAsync(int pageNumber, int pageSize);
        Task<TicketResponseDTO> GetByIdAsync(int id);
        Task<TicketResponseDTO> CreateAsync(TicketDTO ticketDTO);
        Task<TicketResponseDTO> UpdateAsync(int id, TicketDTO ticketDTO);
        Task DeleteAsync(int id);
        Task<CommentResponseDTO> AddCommentAsync(int ticketId, int authorId, string content);
    }
}
