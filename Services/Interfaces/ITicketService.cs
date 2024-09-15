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
        Task<TicketResponseDTO> AssignCustomersAsync(int ticketId, List<int> customerIds);
        Task<TicketResponseDTO> AssignAssigneesAsync(int ticketId, List<int> assigneeIds);
        Task<CommentResponseDTO> AddCommentAsync(int ticketId, int authorId, string content);
        Task<CommentResponseDTO> UpdateCommentAsync(int commentId, string newContent, int currentUserId);
    }
}
