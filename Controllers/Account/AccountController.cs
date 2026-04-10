using System.Security.Claims;
using GlobalFlameMinistry.API.DTOs.Account;
using GlobalFlameMinistry.API.Interfaces.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Account
{
    [Route("api/account")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // GET /api/account/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var profile = await _accountService.GetProfileAsync(UserId);

            if (profile is null)
                return NotFound("User not found");

            return Ok(profile);
        }

        // PUT /api/account/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            try
            {
                var profile = await _accountService.UpdateProfileAsync(UserId, dto);

                if (profile is null)
                    return NotFound("User not found");

                return Ok(profile);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST /api/account/me/profile-picture
        [HttpPost("me/profile-picture")]
        public async Task<IActionResult> UpdateProfilePicture(
            [FromBody] UpdateProfilePictureDto dto)
        {
            var url = await _accountService.UpdateProfilePictureAsync(
                UserId, dto.ProfilePictureUrl);

            if (url is null)
                return NotFound("User not found");

            return Ok(new { profilePictureUrl = url });
        }

        // GET /api/account/me/prayer-requests
        [HttpGet("me/prayer-requests")]
        public async Task<IActionResult> GetMyPrayerRequests()
        {
            var result = await _accountService.GetMyPrayerRequestsAsync(UserId);
            return Ok(result);
        }

        // GET /api/account/me/registrations
        [HttpGet("me/registrations")]
        public async Task<IActionResult> GetMyRegistrations()
        {
            var result = await _accountService.GetMyRegistrationsAsync(UserId);

            return Ok(result);
        }

        // GET /api/account/me/donations
        [HttpGet("me/donations")]
        public async Task<IActionResult> GetMyDonations()
        {
            var result = await _accountService.GetMyDonationsAsync(UserId);

            return Ok(result);
        }

        // EMAIL CHANGE 

        // POST /api/account/me/request-email-change
        [HttpPost("me/request-email-change")]
        public async Task<IActionResult> RequestEmailChange(
            [FromBody] ChangeEmailRequestDto dto)
        {
            try
            {
                await _accountService.RequestEmailChangeAsync(UserId, dto.NewEmail);
                return Ok(new
                {
                    message =
                        $"A confirmation link has been sent to {dto.NewEmail}. " +
                        "Please check your inbox and click the link to confirm " +
                        "your new email address."
                });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST /api/account/me/confirm-email-change
        [HttpPost("me/confirm-email-change")]
        public async Task<IActionResult> ConfirmEmailChange(
            [FromBody] ConfirmEmailChangeDto dto)
        {
            var success = await _accountService.ConfirmEmailChangeAsync(UserId, dto);

            if (!success)
                return BadRequest(
                    "Email change failed. The confirmation link may have expired " +
                    "or already been used. Please request a new one.");

            return Ok(new
            {
                message = "Your email address has been updated successfully."
            });
        }
    }
}