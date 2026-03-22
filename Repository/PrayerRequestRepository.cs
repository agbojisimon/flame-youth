using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.DTOs.PrayerRequest;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Repository
{
    public class PrayerRequestRepository : IPrayerRequestRepository
    {
        private readonly AppDbContext _context;
        public PrayerRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PrayerRequest> CreateAsync(PrayerRequest request)
        {
            await _context.PrayerRequests.AddAsync(request);
            await _context.SaveChangesAsync();

            return request;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.PrayerRequests.AnyAsync(r => r.Id == id);
        }

        public async Task<List<PrayerRequest>> GetAllAsync(PrayerRequestQueryObject query)
        {
            var requests = _context.PrayerRequests.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
                requests = requests.Where(r =>
                    r.Name != null &&
                    r.Name.ToLower().Contains(query.Name.ToLower()));

            if (query.IsAttendedTo.HasValue)
                requests = requests.Where(r => r.IsAttendedTo == query.IsAttendedTo.Value);

            if (query.FromDate.HasValue)
                requests = requests.Where(r => r.CreatedAt >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                requests = requests.Where(r => r.CreatedAt <= query.ToDate.Value);

            requests = query.SortBy?.ToLower() switch
            {
                "name" => query.IsDescending
                    ? requests.OrderByDescending(r => r.Name)
                    : requests.OrderBy(r => r.Name),

                "createdat" => query.IsDescending
                    ? requests.OrderByDescending(r => r.CreatedAt)
                    : requests.OrderBy(r => r.CreatedAt),
                _ => requests.OrderByDescending(r => r.CreatedAt)
            };

            return await requests
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();
        }

        public async Task<PrayerRequest?> GetByIdAsync(int id)
        {
            return await _context.PrayerRequests
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<PrayerRequest?> GetByTokenAsync(string token)
        {
            return await _context.PrayerRequests
                .FirstOrDefaultAsync(r => r.AnonymousToken == token);
        }

        public async Task<int> GetCountAsync(PrayerRequestQueryObject query)
        {
            var requests = _context.PrayerRequests.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
                requests = requests.Where(r =>
                    r.Name != null &&
                    r.Name.ToLower().Contains(query.Name.ToLower()));

            if (query.IsAttendedTo.HasValue)
                requests = requests.Where(r => r.IsAttendedTo == query.IsAttendedTo.Value);

            if (query.FromDate.HasValue)
                requests = requests.Where(r => r.CreatedAt >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                requests = requests.Where(r => r.CreatedAt <= query.ToDate.Value);

            return await requests.CountAsync();
        }

        public async Task<PrayerRequest?> UpdateAsync(int id, UpdatePrayerRequestDto dto)
        {
            var request = await _context.PrayerRequests.FindAsync(id);

            if (request is null)
                return null;

            request.IsAttendedTo = dto.IsAttendedTo;

            await _context.SaveChangesAsync();
            return request;
        }
    }
}