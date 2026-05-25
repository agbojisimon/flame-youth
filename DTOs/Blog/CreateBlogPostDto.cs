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

        public string? VideoUrl { get; set; }

        [Required]
        public string Department { get; set; } = string.Empty;

        public bool IsPublished { get; set; }

        public List<CreateBlogBlockDto> Blocks { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var allowedDepartments = new[]
            {
                "Royal Priesthood",
                "House of Opera",
                "Home of Love",
                "Flame Stars"
            };

            if (string.IsNullOrWhiteSpace(Department) || !allowedDepartments.Contains(Department))
            {
                yield return new ValidationResult(
                    "Department must be one of: Royal Priesthood, House of Opera, Home of Love, Flame Stars.",
                    new[] { nameof(Department) });
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
