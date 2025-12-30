using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace g_flame_youth.DTOs.User
{
    public class AssignRoleDto
    {
        public string userId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}