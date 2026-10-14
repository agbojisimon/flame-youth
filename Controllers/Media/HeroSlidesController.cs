using GlobalFlameMinistry.API.DTOs.HeroSlide;
using GlobalFlameMinistry.API.Exceptions;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GlobalFlameMinistry.API.Controllers.Media
{
    [Route("api/ministry/hero-slides")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("GeneralPolicy")]
    public class HeroSlidesController : ControllerBase
    {
        private readonly IHeroSlideService _service;

        public HeroSlidesController(IHeroSlideService service)
        {
            _service = service;
        }

        // GET /api/media/hero — public, active image slides for the homepage hero
        [AllowAnonymous]
        [HttpGet("/api/media/hero")]
        public async Task<IActionResult> GetActive()
        {
            var slides = await _service.GetActiveAsync();
            return Ok(slides);
        }

        // GET /api/media/clips — public, active video clips
        [AllowAnonymous]
        [HttpGet("/api/media/clips")]
        public async Task<IActionResult> GetClips()
        {
            var clips = await _service.GetClipsAsync();
            return Ok(clips);
        }

        // GET /api/media/clips/featured — public, featured video clips (max 3)
        [AllowAnonymous]
        [HttpGet("/api/media/clips/featured")]
        public async Task<IActionResult> GetFeaturedClips()
        {
            var clips = await _service.GetFeaturedClipsAsync();
            return Ok(clips);
        }

        // GET /api/ministry/hero-slides — admin
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var slides = await _service.GetAllAsync();
            return Ok(slides);
        }

        // POST /api/ministry/hero-slides — admin
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHeroSlideDto dto)
        {
            try
            {
                var slide = await _service.CreateAsync(dto);
                return Ok(slide);
            }
            catch (HeroSlideValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/ministry/hero-slides/5 — admin
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateHeroSlideDto dto)
        {
            try
            {
                var slide = await _service.UpdateAsync(id, dto);
                if (slide is null) return NotFound("Hero slide not found");
                return Ok(slide);
            }
            catch (HeroSlideValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE /api/ministry/hero-slides/5 — admin
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound("Hero slide not found");
            return Ok(new { id });
        }
    }
}