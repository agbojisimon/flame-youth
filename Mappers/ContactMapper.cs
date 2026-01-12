
using g_flame_youth.DTOs.Contact;
using g_flame_youth.Models;
using Microsoft.EntityFrameworkCore.Migrations.Internal;

namespace g_flame_youth.Mappers
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
                Type = contactModel.Type,
                Status = contactModel.Status,
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
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}