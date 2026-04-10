using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Event
{
    public class CreateEventDto : IValidatableObject
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
        public int? MinistryId { get; set; }

        [Required]
        [RegularExpression("^(Ministry|Youth)$",
            ErrorMessage = "Module must be 'Ministry' or 'Youth'")]
        public string Module { get; set; } = "Ministry";
        public bool AcceptsRegistrations { get; set; } = true;
        public bool AcceptsDonations { get; set; } = true;

        [StringLength(200)]
        public string? DonationLabel { get; set; }

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (EndDate <= StartDate)
                yield return new ValidationResult(
                    "EndDate must be after StartDate",
                    new[] { nameof(EndDate) });

            if (StartDate < DateTime.UtcNow.AddMinutes(-5))
                yield return new ValidationResult(
                    "StartDate cannot be in the past",
                    new[] { nameof(StartDate) });
        }
    }
}