using GlobalFlameMinistry.API.DTOs.Blog;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GlobalFlameMinistry.API.Controllers.Admin
{
    [Route("api/admin/blog")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminBlogController : ControllerBase
    {
        private readonly IBlogPostService _blogService;

        public AdminBlogController(IBlogPostService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] BlogQueryObject query)
        {
            var result = await _blogService.GetAllAsync(query);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var blogPost = await _blogService.GetByIdAsync(id);

            if (blogPost is null)
                return NotFound("Blog post not found");

            return Ok(blogPost);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBlogPostDto dto)
        {
            var authorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(authorId))
                return Unauthorized("Unauthorized");

            var created = await _blogService.CreateAsync(dto, authorId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBlogPostDto dto)
        {
            var updated = await _blogService.UpdateAsync(id, dto);

            if (updated is null)
                return NotFound("Blog post not found");

            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _blogService.DeleteAsync(id);

            if (!deleted)
                return NotFound("Blog post not found");

            return NoContent();
        }

        [HttpPatch("{id:int}/publish")]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var toggled = await _blogService.TogglePublishAsync(id);

            if (!toggled)
                return NotFound("Blog post not found");

            return Ok(new { isPublished = toggled });
        }
    }
}
