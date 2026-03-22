using GlobalFlameMinistry.API.DTOs.Event;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Mappers
{
    public static class EventMapper
    {
        public static EventResponseDto ToEventResponseDto(this Event eventModel)
        {
            return new EventResponseDto
            {
                Id = eventModel.Id,
                Title = eventModel.Title,
                Description = eventModel.Description,
                StartDate = eventModel.StartDate,
                EndDate = eventModel.EndDate,
                Location = eventModel.Location,
                ImageUrl = eventModel.ImageUrl,
                Module = eventModel.Module,
                IsCancelled = eventModel.IsCancelled,
                AcceptsRegistrations = eventModel.AcceptsRegistrations,
                AcceptsDonations = eventModel.AcceptsDonations,
                DonationLabel = eventModel.DonationLabel,
                CreatedOn = eventModel.CreatedOn,
                UpdatedOn = eventModel.UpdatedOn
            };
        }
        public static Event ToModel(this CreateEventDto createDto)
        {
            return new Event
            {
                Title = createDto.Title,
                Description = createDto.Description,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                Location = createDto.Location,
                ImageUrl = createDto.ImageUrl,
                Module = createDto.Module,
                AcceptsRegistrations = createDto.AcceptsRegistrations,
                AcceptsDonations = createDto.AcceptsDonations,
                DonationLabel = createDto.DonationLabel,
                CreatedOn = DateTime.UtcNow
            };
        }
        public static void ApplyUpdate(this Event eventModel, UpdateEventDto dto)
        {
            eventModel.Title = dto.Title;
            eventModel.Description = dto.Description;
            eventModel.StartDate = dto.StartDate;
            eventModel.EndDate = dto.EndDate;
            eventModel.Location = dto.Location;
            eventModel.ImageUrl = dto.ImageUrl;
            eventModel.IsCancelled = dto.IsCancelled;
            eventModel.AcceptsRegistrations = dto.AcceptsRegistrations;
            eventModel.AcceptsDonations = dto.AcceptsDonations;
            eventModel.DonationLabel = dto.DonationLabel;
            eventModel.UpdatedOn = DateTime.UtcNow;
        }
        public static List<EventResponseDto> ToDtoList(this IEnumerable<Event> events)
        {
            return events.Select(e => e.ToEventResponseDto()).ToList();
        }
    }
}