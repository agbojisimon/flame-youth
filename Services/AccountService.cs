using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.DTOs.Account;
using GlobalFlameMinistry.API.DTOs.Youth;
using GlobalFlameMinistry.API.Interfaces.Account;
using GlobalFlameMinistry.API.Interfaces.Email;
using GlobalFlameMinistry.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IEmailSender _emailSender;

        public AccountService(UserManager<AppUser> userManager, AppDbContext context, IEmailSender emailSender)
        {
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;
        }

        public async Task<MyProfileDto?> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return MapToProfileDto(user, roles.ToList());
        }

        public async Task<MyProfileDto?> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return null;

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;

            if (!string.IsNullOrWhiteSpace(dto.UserName) &&
                dto.UserName != user.UserName)
            {
                var existing = await _userManager.FindByNameAsync(dto.UserName);
                if (existing is not null && existing.Id != userId)
                    throw new ApplicationException("Username is already taken.");

                var setResult = await _userManager.SetUserNameAsync(user, dto.UserName);
                if (!setResult.Succeeded)
                    throw new ApplicationException(
                        string.Join(", ", setResult.Errors.Select(e => e.Description)));
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ApplicationException(
                    string.Join(", ", result.Errors.Select(e => e.Description)));

            var roles = await _userManager.GetRolesAsync(user);

            return MapToProfileDto(user, roles.ToList());
        }

        public async Task<string?> UpdateProfilePictureAsync(string userId, string profilePictureUrl)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return null;

            user.ProfilePictureUrl = profilePictureUrl;
            await _userManager.UpdateAsync(user);

            return user.ProfilePictureUrl;
        }

        public async Task<List<MyPrayerRequestDto>> GetMyPrayerRequestsAsync(string userId)
        {
            return await _context.PrayerRequests
                .Where(p => p.AppUserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new MyPrayerRequestDto
                {
                    Id = p.Id,
                    Content = p.Content,
                    IsAttendedTo = p.IsAttendedTo,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<MyRegistrationDto>> GetMyRegistrationsAsync(string userId)
        {
            return await _context.EventRegistrations
                .Include(r => r.Event)
                .Where(r => r.AppUserId == userId)
                .OrderByDescending(r => r.RegisteredAt)
                .Select(r => new MyRegistrationDto
                {
                    Id = r.Id,
                    EventId = r.EventId,
                    EventTitle = r.Event.Title,
                    EventLocation = r.Event.Location,
                    EventStartDate = r.Event.StartDate,
                    EventEndDate = r.Event.EndDate,
                    EventImageUrl = r.Event.ImageUrl,
                    EventModule = r.Event.Module,
                    EventIsCancelled = r.Event.IsCancelled,
                    RegisteredAt = r.RegisteredAt
                })
                .ToListAsync();
        }

        public async Task<List<MyDonationDto>> GetMyDonationsAsync(string userId)
        {
            return await _context.Donations
                .Where(d => d.AppUserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new MyDonationDto
                {
                    Id = d.Id,
                    Amount = d.Amount,
                    Currency = d.Currency,
                    DonationType = d.DonationType,
                    PaymentMethod = d.PaymentMethod,
                    Status = d.Status,
                    TransactionReference = d.TransactionReference,
                    EventId = d.EventId,
                    EventTitle = d.EventTitle,
                    CreatedAt = d.CreatedAt
                })
                .ToListAsync();
        }

        // PRIVATE MAPPER 
        private static MyProfileDto MapToProfileDto(AppUser user, List<string> roles)
        {
            return new MyProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Module = user.Module,
                EmailConfirmed = user.EmailConfirmed,
                CreatedOn = user.CreatedOn,
                Roles = roles
            };
        }

        public async Task RequestEmailChangeAsync(string userId, string newEmail, string currentPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new ApplicationException("User not found.");

            if (!await _userManager.CheckPasswordAsync(user, currentPassword))
                throw new ApplicationException("Invalid password. Please enter your current password to change your email.");

            // Check the new email isn't already taken by someone else
            var existing = await _userManager.FindByEmailAsync(newEmail);
            if (existing is not null && existing.Id != userId)
                throw new ApplicationException(
                    "This email address is already in use by another account.");

            // GenerateChangeEmailTokenAsync produces a token tied to BOTH the
            // user AND the new email — it cannot be reused for a different address
            var token = await _userManager.GenerateChangeEmailTokenAsync(
                user, newEmail);

            // URL-encode the token — it contains special characters
            var encodedToken = Uri.EscapeDataString(token);
            var encodedEmail = Uri.EscapeDataString(newEmail);

            var confirmLink =
                $"https://globalflameministry.org/confirm-email-change" +
                $"?token={encodedToken}&newEmail={encodedEmail}";

            var subject = "Confirm your new email address — Global Flame Ministry";
            var body = $@"
                <div style='font-family: Georgia, serif; max-width: 600px;
                    margin: auto; padding: 40px; background: #ffffff;'>
                  <h2 style='color: #0f172a;'>Email Address Change</h2>
                  <p style='color: #475569; font-size: 16px; line-height: 1.7;'>
                    You requested to change your email address on your
                    Global Flame Ministry account to
                    <strong>{newEmail}</strong>.
                  </p>
                  <p style='color: #475569; font-size: 16px; line-height: 1.7;'>
                    Click the button below to confirm this change. This link
                    expires in 1 hour.
                  </p>
                  <div style='text-align: center; margin: 40px 0;'>
                    <a href='{confirmLink}'
                       style='background: #a21caf; color: white; padding: 14px 32px;
                              text-decoration: none; font-weight: bold;
                              border-radius: 8px; font-size: 14px;
                              letter-spacing: 0.1em; text-transform: uppercase;'>
                      Confirm New Email
                    </a>
                  </div>
                  <p style='color: #94a3b8; font-size: 13px;'>
                    If you did not request this change, you can safely ignore
                    this email. Your account remains secure.
                  </p>
                  <hr style='border: none; border-top: 1px solid #e2e8f0;
                      margin: 32px 0;' />
                  <p style='color: #94a3b8; font-size: 13px;'>
                    Global Flame Ministry · Jos, Plateau State, Nigeria
                  </p>
                </div>";

            try
            {
                await _emailSender.SendEmailAsync(newEmail, subject, body);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    "We could not send the confirmation email. Please try again later.", ex);
            }
        }

        // EMAIL CHANGE — STEP 2 
        public async Task<bool> ConfirmEmailChangeAsync(
            string userId, ConfirmEmailChangeDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return false;

            var result = await _userManager.ChangeEmailAsync(
                user, dto.NewEmail, dto.Token);

            if (!result.Succeeded)
                return false;

            if (user.UserName == user.Email ||
                user.UserName == null)
            {
                await _userManager.SetUserNameAsync(user, dto.NewEmail);
            }

            return true;
        }
        // AccountService.cs — add this method
        public async Task<JoinYouthResultDto> JoinYouthCommunityAsync(
            string userId, JoinYouthDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new ApplicationException("User not found.");

            // Already a Youth member? Just return success
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Contains("YouthMember"))
                return new JoinYouthResultDto
                {
                    AutoJoined = true,
                    RequiresVerification = false,
                    Message = "You are already a member of the Youth Community."
                };

            // Check if they changed their name or email
            bool emailChanged = !string.Equals(
                user.Email, dto.Email, StringComparison.OrdinalIgnoreCase);
            bool nameChanged = !string.Equals(
                user.FirstName, dto.FirstName, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(
                user.LastName, dto.LastName, StringComparison.OrdinalIgnoreCase);

            if (emailChanged || nameChanged)
            {
                // Treat as new identity — verification required
                // (You can extend this later to create a separate Youth user or 
                //  trigger email verification flow)
                return new JoinYouthResultDto
                {
                    AutoJoined = false,
                    RequiresVerification = true,
                    Message = "Your details differ from your Ministry account. " +
                              "Please verify your new email to complete registration."
                };
            }

            // Same identity — auto-join, no verification needed
            await _userManager.AddToRoleAsync(user, "YouthMember");

            return new JoinYouthResultDto
            {
                AutoJoined = true,
                RequiresVerification = false,
                Message = "Welcome to the Global Flame Youth Community!"
            };
        }
    }
}
