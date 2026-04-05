using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.DTOs.BulkEmail;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces.BulkEmail;
using GlobalFlameMinistry.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Repository
{
    public class BulkEmailRepository : IBulkEmailRepository
    {
        private readonly AppDbContext _context;

        public BulkEmailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BulkEmailMessage> CreateAsync(BulkEmailMessage message)
        {
            await _context.BulkEmailMessages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<BulkEmailMessage?> GetByIdAsync(int id)
        {
            return await _context.BulkEmailMessages
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<BulkEmailMessage>> GetAllAsync(BulkEmailQueryObject query)
        {
            var messages = _context.BulkEmailMessages.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Subject))
                messages = messages.Where(e =>
                    e.Subject.ToLower().Contains(query.Subject.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.Status))
                messages = messages.Where(e => e.Status == query.Status);

            if (!string.IsNullOrWhiteSpace(query.TargetGroup))
                messages = messages.Where(e => e.TargetGroup == query.TargetGroup);

            if (query.FromDate.HasValue)
                messages = messages.Where(e => e.CreatedOn >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                messages = messages.Where(e => e.CreatedOn <= query.ToDate.Value);

            messages = query.SortBy?.ToLower() switch
            {
                "subject" => query.IsDescending
                    ? messages.OrderByDescending(e => e.Subject)
                    : messages.OrderBy(e => e.Subject),

                "status" => query.IsDescending
                    ? messages.OrderByDescending(e => e.Status)
                    : messages.OrderBy(e => e.Status),

                "recipients" => query.IsDescending
                    ? messages.OrderByDescending(e => e.TotalRecipients)
                    : messages.OrderBy(e => e.TotalRecipients),

                _ => messages.OrderByDescending(e => e.CreatedOn)
            };

            return await messages
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(BulkEmailQueryObject query)
        {
            var messages = _context.BulkEmailMessages.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Subject))
                messages = messages.Where(e =>
                    e.Subject.ToLower().Contains(query.Subject.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.Status))
                messages = messages.Where(e => e.Status == query.Status);

            if (!string.IsNullOrWhiteSpace(query.TargetGroup))
                messages = messages.Where(e => e.TargetGroup == query.TargetGroup);

            if (query.FromDate.HasValue)
                messages = messages.Where(e => e.CreatedOn >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                messages = messages.Where(e => e.CreatedOn <= query.ToDate.Value);

            return await messages.CountAsync();
        }

        public async Task<List<BulkEmailMessage>> GetDueScheduledAsync()
        {
            return await _context.BulkEmailMessages
                .Where(e =>
                    e.Status == "Scheduled" &&
                    e.ScheduledAt.HasValue &&
                    e.ScheduledAt.Value <= DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<BulkEmailMessage?> UpdateAsync(BulkEmailMessage message)
        {
            _context.BulkEmailMessages.Update(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<bool> CancelAsync(int id)
        {
            var message = await _context.BulkEmailMessages
                .FirstOrDefaultAsync(e => e.Id == id && e.Status == "Scheduled");

            if (message is null) return false;

            message.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return true;
        }

        // ── FIXED: database-side aggregation, no full table load ──────────────
        public async Task<BulkEmailStatsDto> GetStatsAsync()
        {
            var totalSent = await _context.BulkEmailMessages
                .CountAsync(e => e.Status == "Sent");
            var totalScheduled = await _context.BulkEmailMessages
                .CountAsync(e => e.Status == "Scheduled");
            var totalFailed = await _context.BulkEmailMessages
                .CountAsync(e => e.Status == "Failed");

            var totals = await _context.BulkEmailMessages
                .Where(e => e.Status == "Sent")
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    TotalRecipients = g.Sum(e => e.TotalRecipients),
                    SuccessCount = g.Sum(e => e.SuccessCount),
                })
                .FirstOrDefaultAsync();

            var totalRecipients = totals?.TotalRecipients ?? 0;
            var successCount = totals?.SuccessCount ?? 0;

            return new BulkEmailStatsDto
            {
                TotalEmailsSent = totalSent,
                TotalRecipientsReached = totalRecipients,
                TotalScheduled = totalScheduled,
                TotalFailed = totalFailed,
                SuccessRate = totalRecipients == 0 ? 0 :
                    Math.Round((double)successCount / totalRecipients * 100, 1),
            };
        }
    }
}