using Api.Models;

namespace Api.Repositories
{
    public interface ITicketRepository
    {
        Task<Ticket> GetByIdAsync(int id);
        Task<IEnumerable<Ticket>> GetAllAsync();
        Task AddAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task DeleteAsync(int id);
        Task<Agency> GetAgencyByIdAsync(int id);
        Task<List<User>> GetUsersByIdsAsync(IEnumerable<int> ids);

        Task AddCommentAsync(Comment comment);
    }
}
