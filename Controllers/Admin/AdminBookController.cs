using GlobalFlameMinistry.API.DTOs.Books;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Admin
{
    [Route("api/admin/books")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminBookController : ControllerBase
    {
        private readonly IBookService _service;

        public AdminBookController(IBookService service)
        {
            _service = service;
        }

        // GET /api/admin/books
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] BookQueryObject query)
        {
            var result = await _service.GetAllAsync(query);

            return Ok(result);
        }

        // GET /api/admin/books/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _service.GetByIdAsync(id);

            if (book is null)
                return NotFound("Book not found");

            return Ok(book);
        }

        // POST /api/admin/books
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookDto dto)
        {
            var book = await _service.CreateAsync(dto);

            return Ok(new { isSuccess = true, data = book });
        }

        // PUT /api/admin/books/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id, [FromBody] UpdateBookDto dto)
        {
            var book = await _service.UpdateAsync(id, dto);

            if (book is null)
                return NotFound("Book not found");

            return Ok(new { isSuccess = true, data = book });
        }

        // DELETE /api/admin/books/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound("Book not found");

            return Ok(new { isSuccess = true });
        }
    }
}