using g_flame_youth.DTOs.Testimony;
using g_flame_youth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace g_flame_youth.Controllers.Members
{
    [Route("api/public/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PublicTestimonyController : ControllerBase
    {
        private readonly ITestimonyService _testimonyService;
        public PublicTestimonyController(ITestimonyService testimonyService)
        {
            _testimonyService = testimonyService;
        }

        [HttpPost("create-testimony")]
        public async Task<IActionResult> CreateTestimony([FromBody] CreateTestimonyDto createDto, string userId)
        {
            var testimony = await _testimonyService.CreateTestimonyAsync(createDto, userId);

            if (testimony == null)
            {
                return BadRequest(new ApiResponse<TestimonyResponseDto?>
                {
                    isSuccess = false,
                    Message = "Failed to create testimony",
                    Data = null
                });
            }

            return Ok(new ApiResponse<TestimonyResponseDto>
            {
                isSuccess = true,
                Message = "Testimony created successfully",
                Data = testimony
            });
        }
    }
}