using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.DTOs.Ministry;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces.Ministry;
using GlobalFlameMinistry.API.Mappers;
using GlobalFlameMinistry.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Repository
{
    public class MinistryRepository : IMinistryRepository
    {
        private readonly AppDbContext _context;

        public MinistryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MinistryDepartment>> GetAllAsync(MinistryQueryObject query)
        {
            var ministries = _context.MinistryDepartments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
                ministries =
                ministries.Where(m => m.Name.ToLower().Contains(query.Name.ToLower()));

            if (query.IsPublished.HasValue)
                ministries =
                ministries.Where(m => m.IsPublished == query.IsPublished.Value);

            ministries = query.SortBy?.ToLower() switch
            {
                "name" => query.IsDescending
                    ? ministries.OrderByDescending(m => m.Name)
                    : ministries.OrderBy(m => m.Name),

                "createdon" => query.IsDescending
                    ? ministries.OrderByDescending(m => m.CreatedOn)
                    : ministries.OrderBy(m => m.CreatedOn),

                _ => ministries.OrderBy(m => m.DisplayOrder)
                               .ThenBy(m => m.Name)
            };

            return await ministries
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(MinistryQueryObject query)
        {
            var ministries = _context.MinistryDepartments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
                ministries =
                ministries.Where(m => m.Name.ToLower().Contains(query.Name.ToLower()));

            if (query.IsPublished.HasValue)
                ministries =
                ministries.Where(m => m.IsPublished == query.IsPublished.Value);

            return await ministries.CountAsync();
        }

        public async Task<MinistryDepartment?> GetByIdAsync(int id)
        {
            return await _context.MinistryDepartments.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MinistryDepartment?> GetBySlugAsync(string slug)
        {
            return await _context.MinistryDepartments.FirstOrDefaultAsync(m => m.Slug == slug);
        }

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
        {
            var query = _context.MinistryDepartments
                .Where(m => m.Slug == slug);

            if (excludeId.HasValue)
                query = query.Where(m => m.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<MinistryDepartment> CreateAsync(MinistryDepartment ministry)
        {
            await _context.MinistryDepartments.AddAsync(ministry);

            await _context.SaveChangesAsync();

            return ministry;
        }

        public async Task<MinistryDepartment?> UpdateAsync(
            int id, UpdateMinistryDto dto)
        {
            var ministry = await _context.MinistryDepartments.FindAsync(id);

            if (ministry is null)
                return null;

            ministry.ApplyUpdate(dto);

            await _context.SaveChangesAsync();

            return ministry;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ministry = await _context.MinistryDepartments.FindAsync(id);

            if (ministry is null)
                return false;

            // Hard delete — permanently removes from DB
            _context.MinistryDepartments.Remove(ministry);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}