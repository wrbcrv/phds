using Api.Data;
using Api.DTOs;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Ticket> GetByIdAsync(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Location)
                .Include(t => t.Customers)
                .Include(t => t.Assignees)
                .Include(t => t.Comments)
                .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket != null)
            {
                await LoadAgencyHierarchyAsync(ticket.Location);
                foreach (var customer in ticket.Customers)
                {
                    await LoadAgencyHierarchyAsync(customer.Agency);
                }
                foreach (var assignee in ticket.Assignees)
                {
                    await LoadAgencyHierarchyAsync(assignee.Agency);
                }
            }

            return ticket;
        }

        public async Task<PagedResponseDTO<Ticket>> GetAllAsync(int page, int size, TicketFilter filter)
        {
            IQueryable<Ticket> query = _context.Tickets
                .Include(t => t.Location)
                .Include(t => t.Customers)
                .Include(t => t.Assignees)
                .Include(t => t.Comments)
                .ThenInclude(c => c.Author)
                .OrderByDescending(t => t.CreatedAt);

            if (filter != null)
            {
                if (filter.Status.HasValue)
                {
                    query = query.Where(t => t.Status == filter.Status.Value);
                }

                if (filter.Priority.HasValue)
                {
                    query = query.Where(t => t.Priority == filter.Priority.Value);
                }

                if (filter.CreatedAfter.HasValue)
                {
                    query = query.Where(t => t.CreatedAt >= filter.CreatedAfter.Value);
                }

                if (filter.CreatedBefore.HasValue)
                {
                    query = query.Where(t => t.CreatedAt <= filter.CreatedBefore.Value);
                }

                if (!string.IsNullOrEmpty(filter.Subject))
                {
                    query = query.Where(t => t.Subject.Contains(filter.Subject));
                }

                if (!string.IsNullOrEmpty(filter.CustomerName))
                {
                    query = query.Where(t => t.Customers.Any(c => c.FullName.Contains(filter.CustomerName)));
                }

                if (filter.LocationId.HasValue)
                {
                    query = query.Where(t => t.AgencyId == filter.LocationId.Value);
                }
            }

            var totalTickets = await query.CountAsync();

            var tickets = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            foreach (var ticket in tickets)
            {
                await LoadAgencyHierarchyAsync(ticket.Location);
                foreach (var customer in ticket.Customers)
                {
                    await LoadAgencyHierarchyAsync(customer.Agency);
                }
                foreach (var assignee in ticket.Assignees)
                {
                    await LoadAgencyHierarchyAsync(assignee.Agency);
                }
            }

            return new PagedResponseDTO<Ticket>
            {
                Items = tickets,
                Total = totalTickets
            };
        }

        public async Task AddAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Agency> GetAgencyByIdAsync(int id)
        {
            return await _context.Agencies.FindAsync(id);
        }

        public async Task<List<User>> GetUsersByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.Users.Where(u => ids.Contains(u.Id)).ToListAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(int commentId)
        {
            return await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Ticket)
                .FirstOrDefaultAsync(c => c.Id == commentId);
        }
        
        public async Task AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
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
