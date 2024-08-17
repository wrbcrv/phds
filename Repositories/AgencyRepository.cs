using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class AgencyRepository : IAgencyRepository
    {
        private readonly AppDbContext _context;

        public AgencyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Agency>> GetAllAsync()
        {
            return await _context.Agencies.ToListAsync();
        }

        public async Task<Agency> GetByIdAsync(int id)
        {
            return await _context.Agencies.FindAsync(id);
        }

        public async Task AddAsync(Agency agency)
        {
            await _context.Agencies.AddAsync(agency);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Agency agency)
        {
            _context.Agencies.Update(agency);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var agency = await GetByIdAsync(id);
            if (agency != null)
            {
                _context.Agencies.Remove(agency);
                await _context.SaveChangesAsync();
            }
        }
    }
}
