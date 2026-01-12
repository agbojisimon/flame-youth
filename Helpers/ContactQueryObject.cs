using g_flame_youth.Helpers.Queries;
using g_flame_youth.Models;

namespace g_flame_youth.Helpers
{
    public class ContactQueryObject : BaseQueryObject
    {
        public string? FullName { get; set; } = null;
        public string? Message { get; set; } = null;
        public string? Email { get; set; } = null;
        public string? PhoneNumber { get; set; } = null;
        public ContactMessageType? Type { get; set; }
        public ContactMessageStatus? Status { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
    }
}