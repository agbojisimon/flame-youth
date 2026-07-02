
namespace GlobalFlameMinistry.API.Helpers.Queries
{
    public class BaseQueryObject
    {
        private int _pageNumber = 1;
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 10 : value > 100 ? 100 : value;
        }
        public bool IsDescending { get; set; } = false;
        public string? SortBy { get; set; } = null;
    }
}