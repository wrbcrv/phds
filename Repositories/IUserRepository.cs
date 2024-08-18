using Api.Models;

namespace Api.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<List<User>> FindByFullNameAsync(string fullName);
        Task<User> FindByUsernameAsync(string username);
    }
}