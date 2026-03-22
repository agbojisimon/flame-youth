using GlobalFlameMinistry.API.DTOs.Contact;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IContactRepository
    {
        Task<List<Contact>> GetAllAsync(ContactQueryObject query);
        Task<int> GetCountAsync(ContactQueryObject query);
        Task<Contact?> GetByIdAsync(int id);
        Task<Contact> CreateAsync(Contact contact);
        Task<Contact?> UpdateStatusAsync(int id, UpdateContactDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}