using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Sermon;
using GlobalFlameMinistry.API.Helpers.Queries;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface ISermonService
    {
        // Public
        Task<PagedResult<SermonResponseDto>> GetPublishedAsync(SermonQueryObject query);
        Task<SermonResponseDto?> GetByIdAsync(int id);

        // Admin
        Task<PagedResult<SermonResponseDto>> GetAllAsync(SermonQueryObject query);
        Task<SermonResponseDto> CreateAsync(CreateSermonDto dto);
        Task<SermonResponseDto?> UpdateAsync(int id, UpdateSermonDto dto);
        Task<bool> DeleteAsync(int id);
    }
}