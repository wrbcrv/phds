using Api.Models;

namespace Api.DTOs
{
    public class NotificationResponseDTO
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }

        public static NotificationResponseDTO ValueOf(Notification notification)
        {
            return new NotificationResponseDTO
            {
                Id = notification.Id,
                Message = notification.Message,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead
            };
        }
    }
}
