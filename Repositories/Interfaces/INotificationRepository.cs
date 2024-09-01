using Api.Models;

namespace Api.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task DeleteAsync(int id);
    }
}
