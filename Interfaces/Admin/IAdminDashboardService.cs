using GlobalFlameMinistry.API.DTOs.Admin;

namespace GlobalFlameMinistry.API.Interfaces.Admin
{
    public interface IAdminDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}