using GlobalFlameMinistry.API.DTOs.Books;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Mappers
{
    public static class BookMapper
    {
        public static BookResponseDto ToDto(this Book bookModel)
        {
            return new BookResponseDto
            {
                Id = bookModel.Id,
                Title = bookModel.Title,
                Author = bookModel.Author,
                Description = bookModel.Description,
                CoverImageUrl = bookModel.CoverImageUrl,
                AmazonUrl = bookModel.AmazonUrl,
                SelarUrl = bookModel.SelarUrl,
                Price = bookModel.Price,
                Currency = bookModel.Currency,
                IsFeatured = bookModel.IsFeatured,
                IsPublished = bookModel.IsPublished,
                CreatedOn = bookModel.CreatedOn,
                UpdatedOn = bookModel.UpdatedOn,
            };
        }

        public static Book ToModel(this CreateBookDto createDto)
        {
            return new Book
            {
                Title = createDto.Title,
                Author = createDto.Author,
                Description = createDto.Description,
                CoverImageUrl = createDto.CoverImageUrl,
                AmazonUrl = createDto.AmazonUrl,
                SelarUrl = createDto.SelarUrl,
                Price = createDto.Price,
                Currency = createDto.Currency,
                IsFeatured = createDto.IsFeatured,
                IsPublished = createDto.IsPublished,
                CreatedOn = DateTime.UtcNow,
            };
        }

        public static void ApplyUpdate(this Book book, UpdateBookDto updateDto)
        {
            book.Title = updateDto.Title;
            book.Author = updateDto.Author;
            book.Description = updateDto.Description;
            book.CoverImageUrl = updateDto.CoverImageUrl;
            book.AmazonUrl = updateDto.AmazonUrl;
            book.SelarUrl = updateDto.SelarUrl;
            book.Price = updateDto.Price;
            book.Currency = updateDto.Currency;
            book.IsFeatured = updateDto.IsFeatured;
            book.IsPublished = updateDto.IsPublished;
            book.UpdatedOn = DateTime.UtcNow;
        }

        public static List<BookResponseDto> ToDtoList(
            this IEnumerable<Book> books)
        {
            return books.Select(b => b.ToDto()).ToList();
        }
    }
}