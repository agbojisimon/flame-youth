using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Event
{
    public class CreateEventDto : IValidatableObject
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(5000)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(300)]
        public string Location { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ImageUrl { get; set; }
        public int? MinistryId { get; set; }

        [Required]
        [RegularExpression("^(Ministry|Youth)$",
            ErrorMessage = "Module must be 'Ministry' or 'Youth'")]
        public string Module { get; set; } = "Ministry";
        public bool AcceptsRegistrations { get; set; } = true;

        [Range(0, 100000, ErrorMessage = "Max registrations must be between 0 and 100000")]
        public int? MaxRegistrations { get; set; }
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