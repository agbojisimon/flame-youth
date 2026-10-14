using System.ComponentModel.DataAnnotations;
using GlobalFlameMinistry.API.DTOs.Blog;

namespace GlobalFlameMinistry.API.DTOs.HeroSlide
{
    /// <summary>
    /// Shared validation for the write-up blocks attached to a clip, reusing the
    /// blog's <see cref="CreateBlogBlockDto"/> validation rules.
    /// </summary>
    internal static class HeroSlideBlockValidation
    {
        public static IEnumerable<ValidationResult> ValidateBlocks(List<CreateBlogBlockDto>? blocks)
        {
            if (blocks is null)
            {
                yield break;
            }

            for (var index = 0; index < blocks.Count; index++)
            {
                var block = blocks[index];
                var context = new ValidationContext(block);
                var results = new List<ValidationResult>();

                Validator.TryValidateObject(block, context, results, true);

                foreach (var innerResult in results)
                {
                    yield return new ValidationResult(
                        innerResult.ErrorMessage ?? string.Empty,
                        innerResult.MemberNames.Select(name => $"ContentBlocks[{index}].{name}"));
                }
            }
        }
    }
}
