using g_flame_youth.DTOs.Devotional;
using g_flame_youth.Helpers.Queries;
using g_flame_youth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace g_flame_youth.Controllers.Members
{
    [Route("api/public/devotionals")]
    [ApiController]
    [AllowAnonymous]
    public class PublicDevotionalController : ControllerBase
    {
        private readonly IDevotionalService _devoService;
        public PublicDevotionalController(IDevotionalService devoService)
        {
            _devoService = devoService;
        }

        [HttpGet("today")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTodayDevotional()
        {
            var devotional = await _devoService.GetTodayDevotionalAsync();

            if (devotional == null)
            {
                return Ok(new ApiResponse<DevotionalResponseDto?>
                {
                    isSuccess = true,
                    Message = "No devotional is available for today",
                    Data = null
                });
            }

            return Ok(new ApiResponse<DevotionalResponseDto>
            {
                isSuccess = true,
                Message = "Today's devotional retrieved successfully",
                Data = devotional
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublishedDevotionals([FromQuery] DevotionalQueryObject query)
        {
            var devotionals = await _devoService.GetPublishedDevotionalsAsync(query);

            return Ok(new ApiResponse<List<DevotionalResponseDto>>
            {
                isSuccess = true,
                Message = devotionals.Count == 0
                    ? "No devotional is available at the moment"
                    : "Devotionals retrieved successfully",
                Data = devotionals
            });
        }
    }
}