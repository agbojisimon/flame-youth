using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using g_flame_youth.Models;

namespace g_flame_youth.Interfaces
{
    public interface IAnnouncementRepository
    {
        Task<List<Announcement>> GetAnnouncementsAsync();
        Task<Announcement?> GetAnnouncementByIdAsync(int Id);
    }
}