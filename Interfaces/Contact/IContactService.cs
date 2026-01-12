
using g_flame_youth.DTOs.Contact;
using g_flame_youth.Helpers;

namespace g_flame_youth.Interfaces
{
    public interface IContactService
    {
        Task<List<ContactResponseDto>> GetContactsAsync(ContactQueryObject query);
        Task<ContactResponseDto?> GetContactByIdAsync(int Id);
        Task<ContactResponseDto> CreateContactAsync(CreateContactDto createDto);
        Task<bool> DeleteContactAsync(int Id);
    }
}