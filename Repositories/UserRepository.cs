using Api.Data;
using Api.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _context.Users
            .Include(u => u.Agency)
            .Include(u => u.AssignedTickets)
            .Include(u => u.Notifications)
            .ToListAsync();

            foreach (var user in users)
            {
                await LoadAgencyHierarchyAsync(user.Agency);
            }

            return users;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Agency)
                .Include(u => u.AssignedTickets)
                .Include(u => u.Notifications)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                await LoadAgencyHierarchyAsync(user.Agency);
            }

            return user;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await GetByIdAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> FindByFullNameAsync(string fullName)
        {
            var users = await _context.Users
                .Include(u => u.Agency)
                .Include(u => u.AssignedTickets)
                .Include(u => u.Notifications)
                .Where(u => EF.Functions.Like(u.FullName.ToLower(), $"%{fullName.ToLower()}%"))
                .ToListAsync();

            foreach (var user in users)
            {
                await LoadAgencyHierarchyAsync(user.Agency);
            }

            return users;
        }

        public async Task<User> FindByUsernameAsync(string username)
        {
            var user = await _context.Users
                .Include(u => u.Agency)
                .Include(u => u.AssignedTickets)
                .Include(u => u.Notifications)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user != null)
            {
                await LoadAgencyHierarchyAsync(user.Agency);
            }

            return user;
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            var user = await _context.Users
                .Include(u => u.Agency)
                .Include(u => u.AssignedTickets)
                .Include(u => u.Notifications)
                .SingleOrDefaultAsync(u => u.Email == email);

            if (user != null && user.Agency != null)
            {
                await LoadAgencyHierarchyAsync(user.Agency);
            }

            return user;
        }

        private async Task LoadAgencyHierarchyAsync(Agency agency)
        {
            if (agency == null || agency.ParentId == null)
                return;

            agency.Parent = await _context.Agencies
                .Include(a => a.Parent)
                .FirstOrDefaultAsync(a => a.Id == agency.ParentId);

            await LoadAgencyHierarchyAsync(agency.Parent);
        }
    }
}
