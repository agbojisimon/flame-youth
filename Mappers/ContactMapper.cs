
using GlobalFlameMinistry.API.DTOs.Contact;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Mappers
{
    public static class ContactMapper
    {
        public static ContactResponseDto ToContactResponsDto(this Contact contactModel)
        {
            return new ContactResponseDto
            {
                Id = contactModel.Id,
                FullName = contactModel.FullName,
                Email = contactModel.Email,
                PhoneNumber = contactModel.PhoneNumber,
                Message = contactModel.Message,
                Type = contactModel.Type.ToString(),
                Status = contactModel.Status.ToString(),
                CreatedAt = contactModel.CreatedAt
            };
        }

        public static Contact ToContactFromCreateDto(this CreateContactDto createDto)
        {
            return new Contact
            {
                FullName = createDto.FullName,
                Email = createDto.Email,
                PhoneNumber = createDto.PhoneNumber,
                Message = createDto.Message,
                Type = createDto.Type,
                Status = ContactMessageStatus.New,
                CreatedAt = DateTime.UtcNow
            };
        }
        public static List<ContactResponseDto> ToDtoList(this IEnumerable<Contact> contacts)
        {
            return contacts.Select(c => c.ToContactResponsDto()).ToList();
        }
    }
}