using GlobalFlameMinistry.API.DTOs.HeroSlide;
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

        // GET /api/media/hero — public, active slides for the homepage hero
        [AllowAnonymous]
        [HttpGet("/api/media/hero")]
        public async Task<IActionResult> GetActive()
        {
            var slides = await _service.GetActiveAsync();
            return Ok(slides);
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
            var slide = await _service.CreateAsync(dto);
            return Ok(slide);
        }

        // PUT /api/ministry/hero-slides/5 — admin
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateHeroSlideDto dto)
        {
            var slide = await _service.UpdateAsync(id, dto);
            if (slide is null) return NotFound("Hero slide not found");
            return Ok(slide);
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