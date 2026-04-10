using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.DTOs.Event;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using GlobalFlameMinistry.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Repository
{
    public class EventRepository : IEventRepository
    {
        public readonly AppDbContext _context;
        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Event> CreateAsync(Event eventModel)
        {
            await _context.Events.AddAsync(eventModel);
            await _context.SaveChangesAsync();

            return eventModel;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt is null) return false;

            // Soft delete
            evt.IsDeleted = true;
            evt.DeletedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Events.AnyAsync(e => e.Id == id);
        }

        public async Task<List<Event>> GetAllAsync(EventQueryObject query)
        {
            var events = _context.Events.AsQueryable();

            events = events.Include(e => e.Ministry);

            if (!string.IsNullOrWhiteSpace(query.Title))
                events = events.Where(e =>
                    e.Title.ToLower().Contains(query.Title.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.Module))
                events = events.Where(e => e.Module == query.Module);

            if (!string.IsNullOrWhiteSpace(query.Location))
                events = events.Where(e =>
                    e.Location.ToLower().Contains(query.Location.ToLower()));

            if (query.IsCancelled.HasValue)
                events = events.Where(e => e.IsCancelled == query.IsCancelled.Value);

            if (query.UpcomingOnly.HasValue && query.UpcomingOnly.Value)
                events = events.Where(e => e.StartDate > DateTime.UtcNow);

            if (query.OngoingOnly.HasValue && query.OngoingOnly.Value)
                events = events.Where(e =>
                    e.StartDate <= DateTime.UtcNow &&
                    e.EndDate >= DateTime.UtcNow);

            if (query.PastOnly.HasValue && query.PastOnly.Value)
                events = events.Where(e => e.EndDate < DateTime.UtcNow);

            if (query.MinistryId.HasValue)
                events = events.Where(e => e.MinistryId == query.MinistryId.Value);

            if (query.FromDate.HasValue)
                events = events.Where(e => e.StartDate >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                events = events.Where(e => e.StartDate <= query.ToDate.Value);

            events = query.SortBy?.ToLower() switch
            {
                "title" => query.IsDescending
                    ? events.OrderByDescending(e => e.Title)
                    : events.OrderBy(e => e.Title),

                "startdate" => query.IsDescending
                    ? events.OrderByDescending(e => e.StartDate)
                    : events.OrderBy(e => e.StartDate),

                "location" => query.IsDescending
                    ? events.OrderByDescending(e => e.Location)
                    : events.OrderBy(e => e.Location),

                "createdon" => query.IsDescending
                    ? events.OrderByDescending(e => e.CreatedOn)
                    : events.OrderBy(e => e.CreatedOn),

                _ => events.OrderBy(e => e.StartDate)
            };

            return await events
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<int> GetCountAsync(EventQueryObject query)
        {
            var events = _context.Events.AsQueryable();

            events = events.Include(e => e.Ministry);

            if (!string.IsNullOrWhiteSpace(query.Title))
                events = events.Where(e =>
                    e.Title.ToLower().Contains(query.Title.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.Module))
                events = events.Where(e => e.Module == query.Module);

            if (!string.IsNullOrWhiteSpace(query.Location))
                events = events.Where(e =>
                    e.Location.ToLower().Contains(query.Location.ToLower()));

            if (query.MinistryId.HasValue)
                events = events.Where(e => e.MinistryId == query.MinistryId.Value);

            if (query.IsCancelled.HasValue)
                events = events.Where(e => e.IsCancelled == query.IsCancelled.Value);

            if (query.UpcomingOnly.HasValue && query.UpcomingOnly.Value)
                events = events.Where(e => e.StartDate > DateTime.UtcNow);

            if (query.OngoingOnly.HasValue && query.OngoingOnly.Value)
                events = events.Where(e =>
                    e.StartDate <= DateTime.UtcNow &&
                    e.EndDate >= DateTime.UtcNow);

            if (query.PastOnly.HasValue && query.PastOnly.Value)
                events = events.Where(e => e.EndDate < DateTime.UtcNow);

            if (query.FromDate.HasValue)
                events = events.Where(e => e.StartDate >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                events = events.Where(e => e.StartDate <= query.ToDate.Value);

            return await events.CountAsync();
        }

        public async Task<Event?> UpdateAsync(int id, UpdateEventDto updateDto)
        {
            var eventModel = await _context.Events.FindAsync(id);
            if (eventModel is null) return null;

            eventModel.ApplyUpdate(updateDto);
            await _context.SaveChangesAsync();

            return eventModel;
        }
    }
}