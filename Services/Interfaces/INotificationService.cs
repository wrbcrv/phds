using Api.DTOs;
using Api.Models;

namespace Api.Services.Interfaces
{
    public interface INotificationService
    {
        Task AddAsync(Notification notification);
        Task DeleteAsync(int id);
        Task Notify(Ticket ticket, User author, string commentContent);
        Task<IEnumerable<NotificationResponseDTO>> GetByUserIdAsync(int userId);
    }
}
