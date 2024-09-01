using Api.Models;

namespace Api.Repositories
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task DeleteAsync(int id);
    }
}
