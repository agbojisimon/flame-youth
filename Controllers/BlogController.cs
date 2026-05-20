using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers
{
    [Route("api/blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogPostService _blogService;

        public BlogController(IBlogPostService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] BlogQueryObject query)
        {
            query.IsPublished = true;
            var result = await _blogService.GetAllAsync(query);
            return Ok(result);
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var blogPost = await _blogService.GetBySlugAsync(slug);

            if (blogPost is null || !blogPost.IsPublished)
                return NotFound("Blog post not found");

            return Ok(blogPost);
        }
    }
}
