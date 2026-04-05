using GlobalFlameMinistry.API.DTOs.Donation;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Ministry
{
    [Route("api/ministry/donations")]
    [ApiController]
    public class DonationController : ControllerBase
    {
        private readonly IDonationService _service;

        public DonationController(IDonationService service)
        {
            _service = service;
        }

        // POST /api/ministry/donations/paystack
        [HttpPost("paystack")]
        public async Task<IActionResult> InitiatePaystack([FromBody] InitiateDonationDto dto)
        {
            var result = await _service.InitiatePaystackAsync(dto);
            return Ok(result);
        }

        // POST /api/ministry/donations/flutterwave
        [HttpPost("flutterwave")]
        public async Task<IActionResult> InitiateFlutterwave([FromBody] InitiateDonationDto dto)
        {
            var result = await _service.InitiateFlutterwaveAsync(dto);
            return Ok(result);
        }

        // GET /api/ministry/donations/verify/paystack?reference=PSK_xxx
        [HttpGet("verify/paystack")]
        public async Task<IActionResult> VerifyPaystack([FromQuery] string reference)
        {
            var success = await _service.VerifyPaystackAsync(reference);

            return Ok(new
            {
                verified = success,
                message = success
                    ? "Payment verified successfully."
                    : "Payment verification failed."
            });
        }

        // GET /api/ministry/donations/verify/flutterwave?transaction_id=xxx
        [HttpGet("verify/flutterwave")]
        public async Task<IActionResult> VerifyFlutterwave([FromQuery] string transaction_id)
        {
            var success = await _service.VerifyFlutterwaveAsync(transaction_id);

            return Ok(new
            {
                verified = success,
                message = success
                    ? "Payment verified successfully."
                    : "Payment verification failed."
            });
        }
    }
}