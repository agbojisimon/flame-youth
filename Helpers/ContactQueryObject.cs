using GlobalFlameMinistry.API.Helpers.Queries;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Helpers
{
    public class ContactQueryObject : BaseQueryObject
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public ContactMessageType? Type { get; set; }
        public ContactMessageStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}