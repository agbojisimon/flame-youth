using GlobalFlameMinistry.API.DTOs.Books;
using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.Helpers;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IBookService
    {
        Task<BookResponseDto> CreateAsync(CreateBookDto dto);
        Task<BookResponseDto?> GetByIdAsync(int id);
        Task<BookResponseDto?> GetBySlugAsync(string slug);
        Task<PagedResult<BookResponseDto>> GetAllAsync(BookQueryObject query);
        Task<BookResponseDto?> UpdateAsync(int id, UpdateBookDto dto);
        Task<bool> DeleteAsync(int id);
    }
}