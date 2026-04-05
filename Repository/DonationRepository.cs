using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.DTOs.Donation;
using GlobalFlameMinistry.API.Helpers.Queries;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Repositories
{
    public class DonationRepository : IDonationRepository
    {
        private readonly AppDbContext _context;

        public DonationRepository(AppDbContext context)
        {
            _context = context;
        }

        // ── GET ALL ───────────────────────────────────────────────────────────
        public async Task<(IEnumerable<Donation> Items, int TotalCount)> GetAllAsync(DonationQueryObject query)
        {
            var donations = _context.Donations.AsQueryable();

            // ── Filters ───────────────────────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(query.DonorName))
                donations = donations.Where(d =>
                    d.DonorName.ToLower().Contains(query.DonorName.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.DonorEmail))
                donations = donations.Where(d =>
                    d.DonorEmail.ToLower().Contains(query.DonorEmail.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.Status))
                donations = donations.Where(d => d.Status == query.Status);

            if (!string.IsNullOrWhiteSpace(query.DonationType))
                donations = donations.Where(d => d.DonationType == query.DonationType);

            if (!string.IsNullOrWhiteSpace(query.PaymentMethod))
                donations = donations.Where(d => d.PaymentMethod == query.PaymentMethod);

            if (!string.IsNullOrWhiteSpace(query.Currency))
                donations = donations.Where(d => d.Currency == query.Currency);

            if (query.EventId.HasValue)
                donations = donations.Where(d => d.EventId == query.EventId.Value);

            if (query.FromDate.HasValue)
                donations = donations.Where(d => d.CreatedAt >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                donations = donations.Where(d => d.CreatedAt <= query.ToDate.Value);

            // ── Sorting ───────────────────────────────────────────────────────
            donations = query.SortBy?.ToLower() switch
            {
                "amount" => query.IsDescending
                    ? donations.OrderByDescending(d => d.Amount)
                    : donations.OrderBy(d => d.Amount),

                "donorname" => query.IsDescending
                    ? donations.OrderByDescending(d => d.DonorName)
                    : donations.OrderBy(d => d.DonorName),

                "status" => query.IsDescending
                    ? donations.OrderByDescending(d => d.Status)
                    : donations.OrderBy(d => d.Status),

                "donationtype" => query.IsDescending
                    ? donations.OrderByDescending(d => d.DonationType)
                    : donations.OrderBy(d => d.DonationType),

                _ => donations.OrderByDescending(d => d.CreatedAt)
            };

            var totalCount = await donations.CountAsync();

            // ── Pagination ────────────────────────────────────────────────────
            var items = await donations
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        // ── GET BY ID ─────────────────────────────────────────────────────────
        public async Task<Donation?> GetByIdAsync(int id)
        {
            return await _context.Donations
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        // ── SUMMARY ───────────────────────────────────────────────────────────
        public async Task<(decimal TotalAmount, int Completed, int Pending)> GetSummaryAsync()
        {
            var totalAmount = await _context.Donations
                .Where(d => d.Status == "Completed")
                .SumAsync(d => d.Amount);

            var completedCount = await _context.Donations
                .CountAsync(d => d.Status == "Completed");

            var pendingCount = await _context.Donations
                .CountAsync(d => d.Status == "Pending");

            return (totalAmount, completedCount, pendingCount);
        }

        // ── STATS ─────────────────────────────────────────────────────────────
        public async Task<DonationStatsDto> GetStatsAsync()
        {
            var completed = _context.Donations
                .Where(d => d.Status == "Completed");

            var byType = await completed
                .GroupBy(d => d.DonationType)
                .Select(g => new DonationGroupDto
                {
                    GroupKey = g.Key,
                    TotalAmount = g.Sum(d => d.Amount),
                    Count = g.Count(),
                })
                .OrderByDescending(g => g.TotalAmount)
                .ToListAsync();

            var byMethod = await completed
                .GroupBy(d => d.PaymentMethod)
                .Select(g => new DonationGroupDto
                {
                    GroupKey = g.Key,
                    TotalAmount = g.Sum(d => d.Amount),
                    Count = g.Count(),
                })
                .ToListAsync();

            var byCurrency = await completed
                .GroupBy(d => d.Currency)
                .Select(g => new DonationGroupDto
                {
                    GroupKey = g.Key,
                    TotalAmount = g.Sum(d => d.Amount),
                    Count = g.Count(),
                })
                .ToListAsync();

            var grandTotal = await completed.SumAsync(d => d.Amount);

            return new DonationStatsDto
            {
                GrandTotal = grandTotal,
                ByType = byType,
                ByMethod = byMethod,
                ByCurrency = byCurrency,
            };
        }
    }
}