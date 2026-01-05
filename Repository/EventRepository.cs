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
            return await _context.Events.Where(e => !e.IsDeleted).ToListAsync();
        }

        public async Task UpdateEventAsync(Event updatedEvent)
        {
            _context.Events.Update(updatedEvent);

            await _context.SaveChangesAsync();
        }
    }
}