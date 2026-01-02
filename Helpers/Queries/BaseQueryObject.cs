
namespace g_flame_youth.Helpers.Queries
{
    public class BaseQueryObject
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool IsDescending { get; set; } = false;
        public string? SortBy { get; set; } = null;
    }
}