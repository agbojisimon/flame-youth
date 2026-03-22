using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Contact;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Interfaces.Email;
using GlobalFlameMinistry.API.Mappers;

namespace GlobalFlameMinistry.API.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepo;
        private readonly IEmailSender _emailSender;

        public ContactService(IContactRepository contactRepo, IEmailSender emailSender)
        {
            _contactRepo = contactRepo;
            _emailSender = emailSender;
        }

        public async Task<ContactResponseDto> CreateAsync(CreateContactDto dto)
        {
            var contact = dto.ToContactFromCreateDto();
            var created = await _contactRepo.CreateAsync(contact);

            try
            {
                await SendConfirmationEmailAsync(created.Email, created.FullName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ContactService] Failed to send confirmation email: {ex.Message}");
            }

            return created.ToContactResponsDto();
        }

        private async Task SendConfirmationEmailAsync(string toEmail, string fullName)
        {
            var firstName = fullName.Split(' ')[0]; // Just use their first name — feels more personal

            var subject = "We've received your message — Global Flame Ministries";

            var body = $"""
                <div style="font-family: Georgia, serif; max-width: 600px; margin: auto; padding: 40px; background: #ffffff;">
                  
                  <h2 style="color: #0f172a; font-size: 24px; margin-bottom: 8px;">
                    Thank you, {firstName}.
                  </h2>

                  <p style="color: #475569; font-size: 16px; line-height: 1.7;">
                    We have received your message and we will get back to you as soon as possible.
                  </p>

                  <p style="color: #475569; font-size: 16px; line-height: 1.7;">
                    In the meantime, feel free to explore our sermons, upcoming events, or join us for service.
                  </p>

                  <hr style="border: none; border-top: 1px solid #e2e8f0; margin: 32px 0;" />

                  <p style="color: #94a3b8; font-size: 13px;">
                    Global Flame Ministries · Zarmaganda, Diye, Off Rayfield Road, Jos, Plateau State, Nigeria.<br/>
                    Weekends at 9am, 11am & 6pm
                  </p>
                </div>
                """;

            await _emailSender.SendEmailAsync(toEmail, subject, body);
        }

        // ... rest of your methods stay the same
        public async Task<bool> DeleteAsync(int id)
            => await _contactRepo.DeleteAsync(id);

        public async Task<PagedResult<ContactResponseDto>> GetAllAsync(ContactQueryObject query)
        {
            var contacts = await _contactRepo.GetAllAsync(query);
            var totalCount = await _contactRepo.GetCountAsync(query);

            return new PagedResult<ContactResponseDto>
            {
                Items = contacts.ToDtoList(),
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public async Task<ContactResponseDto?> GetByIdAsync(int id)
        {
            var contact = await _contactRepo.GetByIdAsync(id);
            if (contact is null) return null;
            return contact.ToContactResponsDto();
        }

        public async Task<ContactResponseDto?> UpdateStatusAsync(int id, UpdateContactDto dto)
        {
            var updated = await _contactRepo.UpdateStatusAsync(id, dto);
            if (updated is null) return null;
            return updated.ToContactResponsDto();
        }
    }
}