using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.DTOs.Testimony;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Models;
using Microsoft.EntityFrameworkCore;
namespace GlobalFlameMinistry.API.Repositories
{
    public class TestimonyRepository : ITestimonyRepository
    {
        private readonly AppDbContext _context;
        public TestimonyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Testimony>> GetApprovedAsync(TestimonyQueryObject query)
        {
            var testimonies = _context.Testimonies
                .Where(t => t.Status == TestimonyStatus.Approved)
                .AsQueryable();

            testimonies = ApplyFilters(testimonies, query);
            testimonies = ApplySorting(testimonies, query);

            return await testimonies
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetApprovedCountAsync(TestimonyQueryObject query)
        {
            var testimonies = _context.Testimonies
                .Where(t => t.Status == TestimonyStatus.Approved)
                .AsQueryable();

            testimonies = ApplyFilters(testimonies, query);
            return await testimonies.CountAsync();
        }

        public async Task<List<Testimony>> GetAllAsync(TestimonyQueryObject query)
        {
            var testimonies = _context.Testimonies.AsQueryable();

            // Admin can filter by status
            if (query.Status.HasValue)
                testimonies = testimonies.Where(t => t.Status == query.Status.Value);

            testimonies = ApplyFilters(testimonies, query);
            testimonies = ApplySorting(testimonies, query);

            return await testimonies
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetAllCountAsync(TestimonyQueryObject query)
        {
            var testimonies = _context.Testimonies.AsQueryable();

            if (query.Status.HasValue)
                testimonies = testimonies.Where(t => t.Status == query.Status.Value);

            testimonies = ApplyFilters(testimonies, query);
            return await testimonies.CountAsync();
        }

        public async Task<Testimony?> GetByIdAsync(int id)
        {
            return await _context.Testimonies
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Testimony> CreateAsync(Testimony testimony)
        {
            await _context.Testimonies.AddAsync(testimony);
            await _context.SaveChangesAsync();
            return testimony;
        }

        public async Task<Testimony?> UpdateStatusAsync(int id, UpdateTestimonyDto dto)
        {
            var testimony = await _context.Testimonies.FindAsync(id);
            if (testimony is null) return null;

            testimony.Status = dto.Status;
            testimony.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return testimony;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var testimony = await _context.Testimonies.FindAsync(id);
            if (testimony is null) return false;

            // Soft delete
            testimony.IsDeleted = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Testimonies.AnyAsync(t => t.Id == id);
        }

        private static IQueryable<Testimony> ApplyFilters(
            IQueryable<Testimony> query,
            TestimonyQueryObject filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.FullName))
                query = query.Where(t =>
                    t.FullName != null &&
                    t.FullName.ToLower().Contains(filter.FullName.ToLower()));

            if (filter.FromDate.HasValue)
                query = query.Where(t => t.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(t => t.CreatedAt <= filter.ToDate.Value);

            return query;
        }

        private static IQueryable<Testimony> ApplySorting(IQueryable<Testimony> query, TestimonyQueryObject filter)
        {
            return filter.SortBy?.ToLower() switch
            {
                "name" => filter.IsDescending
                    ? query.OrderByDescending(t => t.FullName)
                    : query.OrderBy(t => t.FullName),

                "createdat" => filter.IsDescending
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt),

                "status" => filter.IsDescending
                    ? query.OrderByDescending(t => t.Status)
                    : query.OrderBy(t => t.Status),

                // Default
                _ => query.OrderByDescending(t => t.CreatedAt)
            };
        }
    }
}