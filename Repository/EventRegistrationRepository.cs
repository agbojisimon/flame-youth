using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.Exceptions;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace GlobalFlameMinistry.API.Repositories
{
    public class EventRegistrationRepository : IEventRegistrationRepository
    {
        private readonly AppDbContext _context;

        public EventRegistrationRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Inserts a registration and atomically claims a seat in one transaction.
        /// A duplicate (unique EventId + Email) is surfaced as
        /// <see cref="EventRegistrationConflictException"/>; hitting the event's
        /// <see cref="Event.MaxRegistrations"/> is surfaced as <see cref="EventFullException"/>.
        /// On any failure the transaction rolls back, undoing the seat claim.
        /// </summary>
        public async Task<EventRegistration> CreateAsync(EventRegistration registration)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.EventRegistrations.AddAsync(registration);
                await _context.SaveChangesAsync();

                var claimed = await TryClaimSeatAsync(registration.EventId);
                if (!claimed)
                    throw new EventFullException(
                        "This event has reached its maximum number of registrations.");

                await transaction.CommitAsync();
                return registration;
            }
            catch (DbUpdateException ex)
            {
                if (IsUniqueViolation(ex))
                    throw new EventRegistrationConflictException(
                        "This email is already registered for this event.", ex);

                throw;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<bool> TryClaimSeatAsync(int eventId)
        {
            var affected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE events SET seats_taken = seats_taken + 1 WHERE id = {eventId} AND (max_registrations IS NULL OR seats_taken < max_registrations)");

            return affected > 0;
        }

        private static bool IsUniqueViolation(DbUpdateException ex)
        {
            for (Exception? current = ex; current is not null; current = current.InnerException)
            {
                if (current is PostgresException pg &&
                    pg.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    return true;
                }
            }

            return false;
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