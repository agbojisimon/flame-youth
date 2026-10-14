using GlobalFlameMinistry.API.DTOs.Event;
using GlobalFlameMinistry.API.Helpers;
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
                Slug = eventModel.Slug,
                Title = eventModel.Title,
                Description = eventModel.Description,
                StartDate = eventModel.StartDate,
                EndDate = eventModel.EndDate,
                Location = eventModel.Location,
                ImageUrl = eventModel.ImageUrl,
                MinistryId = eventModel.MinistryId,
                MinistryName = eventModel.Ministry?.Name,
                MinistrySlug = eventModel.Ministry?.Slug,
                Module = eventModel.Module,
                IsCancelled = eventModel.IsCancelled,
                IsFeaturedOnHome = eventModel.IsFeaturedOnHome,
                FeaturedOn = eventModel.FeaturedOn,
                AcceptsRegistrations = eventModel.AcceptsRegistrations,
                MaxRegistrations = eventModel.MaxRegistrations,
                SeatsTaken = eventModel.SeatsTaken,
                CanRegister = eventModel.AcceptsRegistrations &&
                    !eventModel.IsCancelled &&
                    (eventModel.MaxRegistrations is null ||
                     eventModel.SeatsTaken < eventModel.MaxRegistrations),
                AcceptsDonations = eventModel.AcceptsDonations,
                DonationLabel = eventModel.DonationLabel,
                ConferenceTheme = eventModel.ConferenceTheme,
                AnchorScripture = eventModel.AnchorScripture,
                AnchorScriptureReference = eventModel.AnchorScriptureReference,
                CreatedOn = eventModel.CreatedOn,
                UpdatedOn = eventModel.UpdatedOn
            };
        }
        public static Event ToModel(this CreateEventDto createDto)
        {
            return new Event
            {
                Title = createDto.Title,
                Slug = SlugHelper.Generate(createDto.Title, 0),
                Description = createDto.Description,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                Location = createDto.Location,
                ImageUrl = createDto.ImageUrl,
                MinistryId = createDto.MinistryId,
                Module = createDto.Module,
                IsFeaturedOnHome = createDto.IsFeaturedOnHome,
                FeaturedOn = createDto.IsFeaturedOnHome ? DateTime.UtcNow : null,
                AcceptsRegistrations = createDto.AcceptsRegistrations,
                MaxRegistrations = createDto.MaxRegistrations,
                AcceptsDonations = createDto.AcceptsDonations,
                DonationLabel = createDto.DonationLabel,
                ConferenceTheme = createDto.ConferenceTheme,
                AnchorScripture = createDto.AnchorScripture,
                AnchorScriptureReference = createDto.AnchorScriptureReference,
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
            eventModel.MinistryId = dto.MinistryId;
            eventModel.IsCancelled = dto.IsCancelled;
            eventModel.IsFeaturedOnHome = dto.IsFeaturedOnHome;
            eventModel.FeaturedOn = dto.IsFeaturedOnHome
                ? eventModel.FeaturedOn ?? DateTime.UtcNow
                : null;
            eventModel.AcceptsRegistrations = dto.AcceptsRegistrations;
            eventModel.MaxRegistrations = dto.MaxRegistrations;
            eventModel.AcceptsDonations = dto.AcceptsDonations;
            eventModel.DonationLabel = dto.DonationLabel;
            eventModel.ConferenceTheme = dto.ConferenceTheme;
            eventModel.AnchorScripture = dto.AnchorScripture;
            eventModel.AnchorScriptureReference = dto.AnchorScriptureReference;
            eventModel.UpdatedOn = DateTime.UtcNow;
        }
        public static List<EventResponseDto> ToDtoList(this IEnumerable<Event> events)
        {
            return events.Select(e => e.ToEventResponseDto()).ToList();
        }
    }
}