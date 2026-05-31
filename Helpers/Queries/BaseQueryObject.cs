
namespace GlobalFlameMinistry.API.Helpers.Queries
{
    public class BaseQueryObject
    {
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 100 ? 100 : value;
        }
        public bool IsDescending { get; set; } = false;
        public string? SortBy { get; set; } = null;
    }
}