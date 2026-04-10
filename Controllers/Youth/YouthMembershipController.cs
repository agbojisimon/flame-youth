using System.Security.Claims;
using GlobalFlameMinistry.API.DTOs.Youth;
using GlobalFlameMinistry.API.Interfaces.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Youth
{
    // Controllers/Youth/YouthMembershipController.cs
    [Route("api/youth")]
    [ApiController]
    [Authorize]
    public class YouthMembershipController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public YouthMembershipController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // POST /api/youth/join
        [HttpPost("join")]
        public async Task<IActionResult> JoinYouthCommunity([FromBody] JoinYouthDto dto)
        {
            try
            {
                var result = await _accountService.JoinYouthCommunityAsync(UserId, dto);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}