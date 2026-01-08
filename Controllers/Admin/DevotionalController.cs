using g_flame_youth.DTOs.Devotional;
using g_flame_youth.Helpers.Queries;
using g_flame_youth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace g_flame_youth.Controllers.Admin
{
    [Route("api/admin/devotionals")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DevotionalController : ControllerBase
    {
        private readonly IDevotionalService _devoService;
        public DevotionalController(IDevotionalService devoService)
        {
            _devoService = devoService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDevotional([FromBody] CreateDevotionalDto createDto)
        {
            try
            {
                var devotional = await _devoService.CreateDevotionalAsync(createDto);

                return Ok(new ApiResponse<DevotionalResponseDto>
                {
                    isSuccess = true,
                    Message = "Devotional created successfully",
                    Data = devotional
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<DevotionalResponseDto?>
                {
                    isSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("preview")]
        public async Task<IActionResult> PreviewDevotionals([FromQuery] DevotionalQueryObject query)
        {
            try
            {
                var devotionals = await _devoService.PreviewDevotionalsAsync(query);

                return Ok(new ApiResponse<List<DevotionalResponseDto>>
                {
                    isSuccess = true,
                    Message = devotionals.Count == 0
                        ? "No devotional is available at the moment"
                        : "Devotionals retrieved successfully",
                    Data = devotionals
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<List<DevotionalResponseDto>?>
                {
                    isSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDevotionalById(int id)
        {
            try
            {
                var devotional = await _devoService.GetDevotionalByIdAsync(id);

                if (devotional == null)
                {
                    return Ok(new ApiResponse<DevotionalResponseDto?>
                    {
                        isSuccess = true,
                        Message = "No devotional is available for this ID",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<DevotionalResponseDto>
                {
                    isSuccess = true,
                    Message = "Devotional retrieved successfully",
                    Data = devotional
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<DevotionalResponseDto?>
                {
                    isSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateDevotional(int id, [FromBody] UpdateDevotionalDto updateDto)
        {
            try
            {
                var devotional = await _devoService.UpdateDevotionalAsync(id, updateDto);

                return Ok(new ApiResponse<DevotionalResponseDto>
                {
                    isSuccess = true,
                    Message = "Devotional updated successfully",
                    Data = devotional
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<DevotionalResponseDto?>
                {
                    isSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDevotional(int id)
        {
            try
            {
                await _devoService.DeleteDevotionalAsync(id);
                return Ok("Devotional deleted successfully");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<string?>
                {
                    isSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPatch("{id:int}/publish")]
        public async Task<IActionResult> PublishDevotional(int id)
        {
            try
            {
                var devotional = await _devoService.PublishDevotionalAsync(id);

                return Ok(new ApiResponse<DevotionalResponseDto>
                {
                    isSuccess = true,
                    Message = "Devotional published successfully",
                    Data = devotional
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<DevotionalResponseDto?>
                {
                    isSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}