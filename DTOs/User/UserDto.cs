using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace g_flame_youth.DTOs.User
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
    }
}