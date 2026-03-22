using GlobalFlameMinistry.API.DTOs.User;
using GlobalFlameMinistry.API.Interfaces.Account;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var result = await _accountService.ChangePasswordAsync(User, dto);

            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfileAsync()
        {
            var result = await _accountService.GetMyProfileAsync(User);

            return Ok(result);
        }

        [HttpPut("update-me")]
        public async Task<IActionResult> UpdateMyProfileAsync([FromBody] UpdateUserDto dto)
        {
            var result = await _accountService.UpdateMyProfileAsync(User, dto);

            return Ok(result);
        }
    }
}