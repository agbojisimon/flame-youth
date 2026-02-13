using g_flame_youth.DTOs.User;
using g_flame_youth.Interfaces.Account;
using Microsoft.AspNetCore.Mvc;

namespace g_flame_youth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfileAsync()
        {
            return Ok(await _accountService.GetMyProfileAsync(User));
        }

        [HttpPut("UpdateMe")]
        public async Task<IActionResult> UpdateMyProfileAsync([FromBody] UpdateUserDto dto)
        {
            return Ok(await _accountService.UpdateMyProfileAsync(User, dto));
        }
    }
}