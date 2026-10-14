using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Blog
{
    public class CreateBlogPostDto : IValidatableObject
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Excerpt { get; set; }

        [MaxLength(500)]
        public string? CoverImageUrl { get; set; }

        [MaxLength(500)]
        public string? VideoUrl { get; set; }

        [Required]
        [MaxLength(50)]
        public string Department { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? ConferenceTheme { get; set; }

        [MaxLength(500)]
        public string? ThemeScripture { get; set; }

        /// <summary>
        /// Optional event start date/time, supplied in UTC by the client.
        /// </summary>
        public DateTime? EventStartsAt { get; set; }

        /// <summary>
        /// Optional event end date/time, supplied in UTC by the client.
        /// </summary>
        public DateTime? EventEndsAt { get; set; }

        public bool IsPublished { get; set; }

        public bool IsFeaturedOnHero { get; set; }

        public List<CreateBlogBlockDto> Blocks { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EventStartsAt.HasValue && EventEndsAt.HasValue && EventEndsAt.Value < EventStartsAt.Value)
            {
                yield return new ValidationResult(
                    "Event end date/time must be on or after the start date/time.",
                    new[] { nameof(EventEndsAt) });
            }

            if (Blocks is null)
            {
                yield return new ValidationResult(
                    "Blocks are required.",
                    new[] { nameof(Blocks) });
            }
            else
            {
                for (var index = 0; index < Blocks.Count; index++)
                {
                    var block = Blocks[index];
                    var context = new ValidationContext(block);
                    var results = new List<ValidationResult>();

                    Validator.TryValidateObject(block, context, results, true);

                    foreach (var innerResult in results)
                    {
                        yield return new ValidationResult(
                            innerResult.ErrorMessage ?? string.Empty,
                            innerResult.MemberNames.Select(name => $"Blocks[{index}].{name}"));
                    }
                }
            }
        }
    }
}