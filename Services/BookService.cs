using GlobalFlameMinistry.API.DTOs.Books;
using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;

namespace GlobalFlameMinistry.API.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repository;

        public BookService(IBookRepository repository)
        {
            _repository = repository;
        }

        public async Task<BookResponseDto> CreateAsync(CreateBookDto dto)
        {
            var book = dto.ToModel();
            var created = await _repository.CreateAsync(book);

            return created.ToDto();
        }

        public async Task<BookResponseDto?> GetByIdAsync(int id)
        {
            var book = await _repository.GetByIdAsync(id);

            return book?.ToDto();
        }

        public async Task<PagedResult<BookResponseDto>> GetAllAsync(
            BookQueryObject query)
        {
            var books = await _repository.GetAllAsync(query);
            var totalCount = await _repository.GetCountAsync(query);

            return new PagedResult<BookResponseDto>
            {
                Items = books.ToDtoList(),
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
            };
        }

        public async Task<BookResponseDto?> UpdateAsync(int id, UpdateBookDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null) return null;

            existing.ApplyUpdate(dto);
            var updated = await _repository.UpdateAsync(id, existing);
            return updated?.ToDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}