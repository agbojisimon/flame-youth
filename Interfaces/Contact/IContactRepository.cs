using g_flame_youth.Helpers;
using g_flame_youth.Models;

namespace g_flame_youth.Interfaces
{
    public interface IContactRepository
    {
        Task<List<Contact>> GetContactsAsync(ContactQueryObject query);
        Task<Contact?> GetContactByIdAsync(int Id);
        Task CreateContactAsync(Contact contact);
        Task<bool> DeleteContactAsync(int Id);
    }
}