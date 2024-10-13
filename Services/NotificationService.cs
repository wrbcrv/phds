using Api.DTOs;
using Api.Models;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;

namespace Api.Services
{
    public class NotificationService(INotificationRepository notificationRepository) : INotificationService
    {
        private readonly INotificationRepository _notificationRepository = notificationRepository;

        public async Task AddAsync(Notification notification)
        {
            await _notificationRepository.AddAsync(notification);
        }

        public async Task DeleteAsync(int id)
        {
            await _notificationRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<NotificationResponseDTO>> GetByUserIdAsync(int userId)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(userId);
            return notifications.Select(NotificationResponseDTO.ValueOf);
        }

        public async Task Notify(Ticket ticket, User author, string commentContent)
        {
            var notifiedUserIds = new HashSet<int>();

            var assigneeNotifications = ticket.Assignees
                .Where(user => user.Id != author.Id)
                .Select(user => new Notification
                {
                    UserId = user.Id,
                    TicketId = ticket.Id,
                    Message = $"Novo comentário de {user.FullName} no chamado #{ticket.Id} – \"{ticket.Subject}\": {commentContent}",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });

            foreach (var notification in assigneeNotifications)
            {
                if (notifiedUserIds.Add(notification.UserId))
                {
                    await AddAsync(notification);
                }
            }

            var customerNotifications = ticket.Customers
                .Where(customer => customer.Id != author.Id)
                .Select(customer => new Notification
                {
                    UserId = customer.Id,
                    TicketId = ticket.Id,
                    Message = $"Novo comentário {customer.FullName} de no chamado #{ticket.Id} – \"{ticket.Subject}\": {commentContent}",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });

            foreach (var notification in customerNotifications)
            {
                if (notifiedUserIds.Add(notification.UserId))
                {
                    await AddAsync(notification);
                }
            }
        }
    }
}
