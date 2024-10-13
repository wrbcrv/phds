using Microsoft.EntityFrameworkCore;
using Api.Models;
using Api.Data;
using Api.Repositories.Interfaces;

namespace Api.Repositories
{
    public class NotificationRepository(AppDbContext context) : INotificationRepository
    {
        private readonly AppDbContext _context = context;

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var notification = await _context.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .ToListAsync();
        }
    }
}
