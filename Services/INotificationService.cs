using Api.Models;

namespace Api.Services
{
    public interface INotificationService
    {
        Task<Notification> GetNotificationByIdAsync(int id);
        Task<IEnumerable<Notification>> GetAllNotificationsAsync();
        Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(int userId);
        Task AddNotificationAsync(Notification notification);
        Task DeleteNotificationAsync(int id);
        Task Notify(Ticket ticket, User author, string commentContent);
    }
}
