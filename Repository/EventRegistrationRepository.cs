using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Repositories
{
    public class EventRegistrationRepository : IEventRegistrationRepository
    {
        private readonly AppDbContext _context;

        public EventRegistrationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EventRegistration> CreateAsync(EventRegistration registration)
        {
            await _context.EventRegistrations.AddAsync(registration);
            await _context.SaveChangesAsync();
            return registration;
        }

        public async Task<List<EventRegistration>> GetByEventIdAsync(int eventId)
        {
            return await _context.EventRegistrations
                .Where(r => r.EventId == eventId)
                .OrderByDescending(r => r.RegisteredAt)
                .ToListAsync();
        }

        public async Task<int> GetCountByEventIdAsync(int eventId)
        {
            return await _context.EventRegistrations
                .CountAsync(r => r.EventId == eventId);
        }

        // Prevent duplicate registrations
        // Real world example: like checking if someone already
        // has a ticket before selling them another one
        public async Task<bool> IsAlreadyRegisteredAsync(int eventId, string email)
        {
            return await _context.EventRegistrations
                .AnyAsync(r => r.EventId == eventId &&
                               r.Email.ToLower() == email.ToLower());
        }
    }
}