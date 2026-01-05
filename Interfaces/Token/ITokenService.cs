using g_flame_youth.Models;

namespace g_flame_youth.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser user);
    }
}