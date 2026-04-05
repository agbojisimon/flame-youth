using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.DTOs.Counselling;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces.Counselling;
using GlobalFlameMinistry.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Repository
{
    public class CounsellingRepository : ICounsellingRepository
    {
        private readonly AppDbContext _context;

        public CounsellingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CounsellingRequest> CreateAsync(CounsellingRequest request)
        {
            await _context.CounsellingRequests.AddAsync(request);

            await _context.SaveChangesAsync();

            return request;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var request = await _context.CounsellingRequests.FindAsync(id);

            if (request is null)
                return false;

            request.IsDeleted = true;
            request.DeletedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<CounsellingRequest>> GetAllAsync(CounsellingQueryObject query)
        {
            var requests = _context.CounsellingRequests.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.FullName))
                requests = requests.Where(c =>
                    c.FullName.ToLower().Contains(query.FullName.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.Email))
                requests = requests.Where(c =>
                    c.Email.ToLower().Contains(query.Email.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.Topic))
                requests = requests.Where(c =>
                    c.Topic.ToLower().Contains(query.Topic.ToLower()));

            if (query.Status.HasValue)
                requests = requests.Where(c => c.Status == query.Status.Value);

            if (query.FromDate.HasValue)
                requests = requests.Where(c => c.CreatedAt >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                requests = requests.Where(c => c.CreatedAt <= query.ToDate.Value);

            requests = query.SortBy?.ToLower() switch
            {
                "fullname" => query.IsDescending
                    ? requests.OrderByDescending(c => c.FullName)
                    : requests.OrderBy(c => c.FullName),

                "status" => query.IsDescending
                    ? requests.OrderByDescending(c => c.Status)
                    : requests.OrderBy(c => c.Status),

                _ => requests.OrderByDescending(c => c.CreatedAt)
            };

            return await requests
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(CounsellingQueryObject query)
        {
            var requests = _context.CounsellingRequests.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.FullName))
                requests = requests.Where(c =>
                    c.FullName.ToLower().Contains(query.FullName.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.Email))
                requests = requests.Where(c =>
                    c.Email.ToLower().Contains(query.Email.ToLower()));

            if (query.Status.HasValue)
                requests = requests.Where(c => c.Status == query.Status.Value);

            if (query.FromDate.HasValue)
                requests = requests.Where(c => c.CreatedAt >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                requests = requests.Where(c => c.CreatedAt <= query.ToDate.Value);

            return await requests.CountAsync();
        }

        public async Task<CounsellingRequest?> GetByIdAsync(int id)
        {
            return await _context.CounsellingRequests.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CounsellingRequest?> AssignAsync(int id, AssignCounsellorDto dto)
        {
            var request = await _context.CounsellingRequests.FindAsync(id);

            if (request is null)
                return null;

            request.AssignedTo = dto.AssignedTo;
            request.AssignedToEmail = dto.AssignedToEmail;
            request.Status = CounsellingStatus.Assigned;
            request.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return request;
        }

        public async Task<CounsellingRequest?> UpdateStatusAsync(int id, CounsellingStatus status)
        {
            var request = await _context.CounsellingRequests.FindAsync(id);

            if (request is null)
                return null;

            request.Status = status;
            request.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return request;
        }
    }
}