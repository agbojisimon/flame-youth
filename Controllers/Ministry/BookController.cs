using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Ministry
{
    [Route("api/ministry/books")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _service;

        public BookController(IBookService service)
        {
            _service = service;
        }

        // GET /api/ministry/books
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] BookQueryObject query)
        {
            // Public only sees published books
            query.IsPublished = true;

            var result = await _service.GetAllAsync(query);

            return Ok(result);
        }

        // GET /api/ministry/books/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _service.GetByIdAsync(id);

            if (book is null || !book.IsPublished)
                return NotFound("Book not found");

            return Ok(book);
        }

        // GET /api/ministry/books/{slug}
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var book = await _service.GetBySlugAsync(slug);
            if (book is null || !book.IsPublished) return NotFound("Book not found");
            return Ok(book);
        }
    }
}