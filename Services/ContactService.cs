using g_flame_youth.DTOs.Contact;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using g_flame_youth.Mappers;

namespace g_flame_youth.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepo;
        public ContactService(IContactRepository contactRepo)
        {
            _contactRepo = contactRepo;
        }

        public async Task<ContactResponseDto> CreateContactAsync(CreateContactDto createDto)
        {
            var contacts = createDto.ToContactFromCreateDto();
            contacts.CreatedAt = DateTime.UtcNow;

            await _contactRepo.CreateContactAsync(contacts);
            return contacts.ToContactResponsDto();
        }

        public async Task<bool> DeleteContactAsync(int Id)
        {
            var contact = await _contactRepo.GetContactByIdAsync(Id);

            if (contact == null)
                return false;

            return await _contactRepo.DeleteContactAsync(Id);
        }

        public async Task<ContactResponseDto?> GetContactByIdAsync(int Id)
        {
            var contact = await _contactRepo.GetContactByIdAsync(Id);

            if (contact == null)
                return null;

            return contact.ToContactResponsDto();
        }

        public async Task<List<ContactResponseDto>> GetContactsAsync(ContactQueryObject query)
        {
            var contacts = await _contactRepo.GetContactsAsync(query);

            return contacts.Select(c => c.ToContactResponsDto()).ToList();
        }
    }
}