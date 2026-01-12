using g_flame_youth.DTOs.Testimony;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace g_flame_youth.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminTestimonyController : ControllerBase
    {
        private readonly ITestimonyService _testimonyService;
        public AdminTestimonyController(ITestimonyService testimonyService)
        {
            _testimonyService = testimonyService;
        }

        [HttpGet("get-all-testimonies")]
        public async Task<IActionResult> GetTestimonies([FromQuery] TestimonyQueryObject query)
        {
            var testimonies = await _testimonyService.GetTestimoniesAsync(query);

            if (testimonies.Count == 0)
            {
                return Ok(new ApiResponse<List<TestimonyResponseDto>?>
                {
                    isSuccess = true,
                    Message = "No testimony is available at the moment",
                    Data = null
                });
            }
            return Ok(new ApiResponse<List<TestimonyResponseDto>>
            {
                isSuccess = true,
                Message = "Testimonies retrieved successfully",
                Data = testimonies,
            });
        }

        [HttpGet("{Id:int}/get-testimony")]
        public async Task<IActionResult> GetTestimony([FromRoute] int Id)
        {
            var testimony = await _testimonyService.GetTestimonyByIdAsync(Id);

            if (testimony == null)
            {
                return Ok(new ApiResponse<TestimonyResponseDto?>
                {
                    isSuccess = false,
                    Message = "Testimony not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<TestimonyResponseDto>
            {
                isSuccess = true,
                Message = "Testimony retrieved successfully",
                Data = testimony
            });
        }

        [HttpDelete("{Id:int}/delete-testimony")]
        public async Task<IActionResult> DeleteTestimony([FromRoute] int Id)
        {
            var isDeleted = await _testimonyService.DeleteTestimonyAsync(Id);

            if (!isDeleted)
            {
                return NotFound(new ApiResponse<bool>
                {
                    isSuccess = false,
                    Message = "Testimony not found or could not be deleted",
                    Data = false
                });
            }

            return Ok(new ApiResponse<bool>
            {
                isSuccess = true,
                Message = "Testimony deleted successfully",
                Data = true
            });
        }
    }
}