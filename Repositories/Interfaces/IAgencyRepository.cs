using Api.Models;

namespace Api.Repositories.Interfaces
{
    public interface IAgencyRepository
    {
        Task<IEnumerable<Agency>> GetAllAsync();
        Task<Agency> GetByIdAsync(int id);
        Task AddAsync(Agency agency);
        Task UpdateAsync(Agency agency);
        Task DeleteAsync(int id);
    }
}
