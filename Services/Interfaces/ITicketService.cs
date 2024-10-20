using Api.DTOs;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

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
        Task<TicketResponseDTO> AssignCustomersAsync(int ticketId, List<int> customerIds);
        Task<TicketResponseDTO> AssignAssigneesAsync(int ticketId, List<int> assigneeIds);
        Task<TicketResponseDTO> RemoveCustomerAsync(int ticketId, int customerId);
        Task<TicketResponseDTO> RemoveAssigneeAsync(int ticketId, int assigneeId);
        Task<CommentResponseDTO> AddCommentAsync(int ticketId, int authorId, string content, IList<IFormFile> files = null);
        Task<CommentResponseDTO> UpdateCommentAsync(int ticketId, int commentId, string newContent);
        Task DeleteCommentAsync(int ticketId, int commentId);
        Task<FileResult> DownloadCommentFileAsync(int ticketId, int commentId);
    }
}
