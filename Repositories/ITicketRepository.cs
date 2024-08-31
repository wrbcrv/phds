using Api.DTOs;
using Api.Models;

namespace Api.Repositories
{
    public interface ITicketRepository
    {
        Task<Ticket> GetByIdAsync(int id);
        Task<PagedResponseDTO<Ticket>> GetAllAsync(int page, int size, TicketFilter filter);
        Task AddAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task DeleteAsync(int id);
        Task<Agency> GetAgencyByIdAsync(int id);
        Task<List<User>> GetUsersByIdsAsync(IEnumerable<int> ids);
        Task<Comment> GetCommentByIdAsync(int commentId);
        Task AddCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);

    }
}
