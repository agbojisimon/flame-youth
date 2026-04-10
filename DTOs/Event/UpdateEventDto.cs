using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Event
{
    public class UpdateEventDto : IValidatableObject
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(300, MinimumLength = 3, ErrorMessage = "Location must be between 3 and 300 characters")]
        public string Location { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string? ImageUrl { get; set; }
        public int? MinistryId { get; set; }
        public bool IsCancelled { get; set; } = false;
        public bool AcceptsRegistrations { get; set; } = true;
        public bool AcceptsDonations { get; set; } = true;

        [StringLength(200)]
        public string? DonationLabel { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate <= StartDate)
            {
                yield return new ValidationResult(
                    "End date must be after start date",
                    new[] { nameof(EndDate) }
                );
            }
        }
    }
}