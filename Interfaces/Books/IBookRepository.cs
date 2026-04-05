using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IBookRepository
    {
        Task<Book> CreateAsync(Book book);
        Task<Book?> GetByIdAsync(int id);
        Task<List<Book>> GetAllAsync(BookQueryObject query);
        Task<int> GetCountAsync(BookQueryObject query);
        Task<Book?> UpdateAsync(int id, Book book);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}