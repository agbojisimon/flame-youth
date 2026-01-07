using g_flame_youth.Data;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using g_flame_youth.Models;
using Microsoft.EntityFrameworkCore;

namespace g_flame_youth.Repository
{
    public class EventRepository : IEvenRepository
    {
        public readonly AppDbContext _context;
        public EventRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateEventAsync(Event newEvent)
        {
            await _context.Events.AddAsync(newEvent);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteEventAsync(int Id)
        {
            var events = await _context.Events.FirstOrDefaultAsync(e => e.Id == Id);

            if (events == null)
                return false;

            events.IsDeleted = true;
            events.DeletedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Event?> GetEventByIdAsync(int Id)
        {
            return await _context.Events.FirstOrDefaultAsync(e => e.Id == Id && !e.IsDeleted);
        }

        public async Task<List<Event>> GetEventsAsync(EventQueryObject query)
        {
            var events = _context.Events.Where(e => !e.IsDeleted && !e.IsCancelled).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                events = events.Where(e => e.Title.Contains(query.Title));
            }
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                events = query.SortBy.Equals("CreatedOn", StringComparison.OrdinalIgnoreCase) ? (query.IsDescending ? events.OrderByDescending(a => a.CreatedOn) : events.OrderBy(a => a.CreatedOn)) : events;
            }
            else
            {
                events = events.OrderByDescending(a => a.CreatedOn);
            }

            var skip = (query.PageNumber - 1) * query.PageSize;

            return await events.Skip(skip).Take(query.PageSize).ToListAsync();
        }

        public async Task UpdateEventAsync(Event updatedEvent)
        {
            _context.Events.Update(updatedEvent);

            await _context.SaveChangesAsync();
        }
    }
}