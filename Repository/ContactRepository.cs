using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.DTOs.Contact;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Repository
{
    public class ContactRepository : IContactRepository
    {
        private readonly AppDbContext _context;
        public ContactRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Contact> CreateAsync(Contact contact)
        {
            await _context.Contacts.AddAsync(contact);

            await _context.SaveChangesAsync();

            return contact;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);

            if (contact is null)
                return false;

            // Soft delete
            contact.IsDeleted = true;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Contacts.AnyAsync(c => c.Id == id);
        }

        public async Task<List<Contact>> GetAllAsync(ContactQueryObject query)
        {
            var contacts = _context.Contacts.AsQueryable();

            // FILTERS 

            if (!string.IsNullOrWhiteSpace(query.FullName))
                contacts = contacts.Where(c =>
                    c.FullName.ToLower().Contains(query.FullName.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.Email))
                contacts = contacts.Where(c =>
                    c.Email.ToLower().Contains(query.Email.ToLower()));

            if (query.Type.HasValue)
                contacts = contacts.Where(c => c.Type == query.Type.Value);

            if (query.Status.HasValue)
                contacts = contacts.Where(c => c.Status == query.Status.Value);

            if (query.FromDate.HasValue)
                contacts = contacts.Where(c => c.CreatedAt >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                contacts = contacts.Where(c => c.CreatedAt <= query.ToDate.Value);

            // SORTING

            contacts = query.SortBy?.ToLower() switch
            {
                "fullname" => query.IsDescending
                    ? contacts.OrderByDescending(c => c.FullName)
                    : contacts.OrderBy(c => c.FullName),

                "email" => query.IsDescending
                    ? contacts.OrderByDescending(c => c.Email)
                    : contacts.OrderBy(c => c.Email),

                "type" => query.IsDescending
                    ? contacts.OrderByDescending(c => c.Type)
                    : contacts.OrderBy(c => c.Type),

                "status" => query.IsDescending
                    ? contacts.OrderByDescending(c => c.Status)
                    : contacts.OrderBy(c => c.Status),

                "createdat" => query.IsDescending
                    ? contacts.OrderByDescending(c => c.CreatedAt)
                    : contacts.OrderBy(c => c.CreatedAt),

                // Default — newest first so admin sees latest messages
                _ => contacts.OrderByDescending(c => c.CreatedAt)
            };

            // PAGINATION
            return await contacts
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();
        }

        public async Task<Contact?> GetByIdAsync(int id)
        {
            return await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> GetCountAsync(ContactQueryObject query)
        {
            var contacts = _context.Contacts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.FullName))
                contacts = contacts.Where(c =>
                    c.FullName.ToLower().Contains(query.FullName.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.Email))
                contacts = contacts.Where(c =>
                    c.Email.ToLower().Contains(query.Email.ToLower()));

            if (query.Type.HasValue)
                contacts = contacts.Where(c => c.Type == query.Type.Value);

            if (query.Status.HasValue)
                contacts = contacts.Where(c => c.Status == query.Status.Value);

            if (query.FromDate.HasValue)
                contacts = contacts.Where(c => c.CreatedAt >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                contacts = contacts.Where(c => c.CreatedAt <= query.ToDate.Value);

            return await contacts.CountAsync();
        }

        public async Task<Contact?> UpdateStatusAsync(int id, UpdateContactDto updateDto)
        {
            var contact = await _context.Contacts.FindAsync(id);

            if (contact is null)
                return null;

            // Only field admin can update is Status
            // New → Read → Responded → Closed
            contact.Status = updateDto.Status;

            await _context.SaveChangesAsync();

            return contact;
        }
    }
}