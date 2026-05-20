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

        [Required]
        public string Module { get; set; } = string.Empty;

        public bool IsPublished { get; set; }

        public List<CreateBlogBlockDto> Blocks { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var allowedModules = new[] { "Ministry", "Youth" };

            if (string.IsNullOrWhiteSpace(Module) || !allowedModules.Contains(Module))
            {
                yield return new ValidationResult(
                    "Module must be either 'Ministry' or 'Youth'.",
                    new[] { nameof(Module) });
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
