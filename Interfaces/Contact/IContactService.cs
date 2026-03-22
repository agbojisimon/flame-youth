using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Contact;
using GlobalFlameMinistry.API.Helpers;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IContactService
    {
        Task<PagedResult<ContactResponseDto>> GetAllAsync(ContactQueryObject query);
        Task<ContactResponseDto?> GetByIdAsync(int id);
        Task<ContactResponseDto?> UpdateStatusAsync(int id, UpdateContactDto dto);
        Task<bool> DeleteAsync(int id);

        // Anyone can contact
        Task<ContactResponseDto> CreateAsync(CreateContactDto dto);
    }
}