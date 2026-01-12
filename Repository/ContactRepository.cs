using g_flame_youth.Data;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using g_flame_youth.Models;
using Microsoft.EntityFrameworkCore;

namespace g_flame_youth.Repository
{
    public class ContactRepository : IContactRepository
    {
        private readonly AppDbContext _context;
        public ContactRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateContactAsync(Contact contact)
        {
            await _context.Contacts.AddAsync(contact);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteContactAsync(int Id)
        {
            var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == Id);

            if (contact == null)
                return false;

            contact.IsDeleted = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Contact?> GetContactByIdAsync(int Id)
        {
            return await _context.Contacts.FirstOrDefaultAsync(c => c.Id == Id && !c.IsDeleted);
        }

        public async Task<List<Contact>> GetContactsAsync(ContactQueryObject query)
        {
            IQueryable<Contact> contacts = _context.Contacts.Where(c => !c.IsDeleted);

            if (!string.IsNullOrWhiteSpace(query.FullName))
            {
                contacts = contacts.Where(c => c.FullName.Contains(query.FullName));
            }

            if (!string.IsNullOrWhiteSpace(query.Email))
            {
                contacts = contacts.Where(c => c.Email.Contains(query.Email));
            }

            if (!string.IsNullOrWhiteSpace(query.PhoneNumber))
            {
                contacts = contacts.Where(c => c.PhoneNumber!.Contains(query.PhoneNumber));
            }

            if (!string.IsNullOrWhiteSpace(query.Message))
            {
                contacts = contacts.Where(c => c.Message.Contains(query.Message));
            }

            if (query.Type.HasValue)
            {
                contacts = contacts.Where(c => c.Type == query.Type.Value);
            }

            if (query.Status.HasValue)
            {
                contacts = contacts.Where(c => c.Status == query.Status.Value);
            }

            if (query.CreatedFrom.HasValue)
            {
                contacts = contacts.Where(c => c.CreatedAt >= query.CreatedFrom.Value);
            }

            if (query.CreatedTo.HasValue)
            {
                contacts = contacts.Where(c => c.CreatedAt <= query.CreatedTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy) &&
                query.SortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
            {
                contacts = query.IsDescending ? contacts.OrderByDescending(c => c.CreatedAt) : contacts.OrderBy(c => c.CreatedAt);
            }
            else
            {
                contacts = contacts.OrderByDescending(c => c.CreatedAt);
            }

            int skip = (query.PageNumber - 1) * query.PageSize;

            return await contacts.Skip(skip).Take(query.PageSize).ToListAsync();
        }
    }
}