using System.Text.RegularExpressions;

namespace GlobalFlameMinistry.API.Helpers
{
    public static class SlugHelper
    {
        public static string Generate(string title, int id = 0)
        {
            var slug = title.ToLowerInvariant();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"-+", "-");
            slug = slug.Trim('-');
            return id > 0 ? $"{slug}-{id}" : slug;
        }
    }
}
