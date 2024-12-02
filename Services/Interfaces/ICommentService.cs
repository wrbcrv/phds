using Api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentResponseDTO>> GetCommentsByTicketIdAsync(int ticketId);
        Task<CommentResponseDTO> AddCommentAsync(int ticketId, int authorId, string content, IList<IFormFile> files = null);
        Task<CommentResponseDTO> UpdateCommentAsync(int ticketId, int commentId, string newContent);
        Task DeleteCommentAsync(int ticketId, int commentId);
    }
}
