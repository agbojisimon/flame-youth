using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Sermon;
using GlobalFlameMinistry.API.Helpers.Queries;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;

namespace GlobalFlameMinistry.API.Services
{
    public class SermonService : ISermonService
    {
        private readonly ISermonRepository _repository;

        public SermonService(ISermonRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<SermonResponseDto>> GetPublishedAsync(
            SermonQueryObject query)
        {
            // Force published only for public
            query.IsPublished = true;
            var sermons = await _repository.GetAllAsync(query);
            var total = await _repository.GetCountAsync(query);

            return new PagedResult<SermonResponseDto>
            {
                Items = sermons.ToDtoList(),
                TotalCount = total,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public async Task<SermonResponseDto?> GetByIdAsync(int id)
        {
            var sermon = await _repository.GetByIdAsync(id);
            if (sermon is null) return null;
            return sermon.ToDto();
        }

        public async Task<PagedResult<SermonResponseDto>> GetAllAsync(
            SermonQueryObject query)
        {
            var sermons = await _repository.GetAllAsync(query);
            var total = await _repository.GetCountAsync(query);

            return new PagedResult<SermonResponseDto>
            {
                Items = sermons.ToDtoList(),
                TotalCount = total,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public async Task<SermonResponseDto> CreateAsync(CreateSermonDto dto)
        {
            var sermon = dto.ToModel();
            var created = await _repository.CreateAsync(sermon);
            return created.ToDto();
        }

        public async Task<SermonResponseDto?> UpdateAsync(int id, UpdateSermonDto dto)
        {
            var updated = await _repository.UpdateAsync(id, dto);
            if (updated is null) return null;
            return updated.ToDto();
        }

        public async Task<bool> DeleteAsync(int id)
            => await _repository.DeleteAsync(id);
    }
}