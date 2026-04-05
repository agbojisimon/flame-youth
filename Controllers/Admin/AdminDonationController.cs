using GlobalFlameMinistry.API.Helpers.Queries;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Admin
{
    [Route("api/admin/donations")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminDonationController : ControllerBase
    {
        private readonly IAdminDonationService _donationService;

        public AdminDonationController(IAdminDonationService donationService)
        {
            _donationService = donationService;
        }

        // GET /api/admin/donations
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DonationQueryObject query)
        {
            var result = await _donationService.GetAllAsync(query);

            return Ok(result);
        }

        // GET /api/admin/donations/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var donation = await _donationService.GetByIdAsync(id);

            if (donation is null)
                return NotFound($"Donation with ID {id} was not found.");

            return Ok(donation);
        }

        // GET /api/admin/donations/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _donationService.GetStatsAsync();

            return Ok(stats);
        }
    }
}