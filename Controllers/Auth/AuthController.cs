using GlobalFlameMinistry.API.DTOs.Account;
using GlobalFlameMinistry.API.DTOs.Auth;
using GlobalFlameMinistry.API.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GlobalFlameMinistry.API.Controllers.Members
{
    [Route("api/auth")]
    [ApiController]
    [EnableRateLimiting("AuthCatchAllPolicy")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [AllowAnonymous]
        [EnableRateLimiting("RegistrationPolicy")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }

        [AllowAnonymous]
        [EnableRateLimiting("LoginPolicy")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            SetRefreshCookie(result.RefreshToken);
            return Ok(new
            {
                accessToken = result.AccessToken,
                user = result.User
            });
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["gfm_refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "No refresh token provided." });

            var result = await _authService.RefreshTokenAsync(refreshToken);
            SetRefreshCookie(result.RefreshToken);
            return Ok(new { accessToken = result.AccessToken });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
                await _authService.LogoutAsync(userId);

            ClearRefreshCookie();
            return Ok(new { message = "Logged out successfully." });
        }

        [AllowAnonymous]
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] EmailConfirmationDto dto)
        {
            var frontendUrl = _config["App:FrontendUrl"];
            try
            {
                await _authService.ConfirmEmailAsync(dto);
                return Redirect($"{frontendUrl}/login?confirmed=true");
            }
            catch
            {
                return Redirect($"{frontendUrl}/resend-confirmation?error=true");
            }
        }

        [AllowAnonymous]
        [EnableRateLimiting("ForgotPasswordPolicy")]
        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation(
            [FromBody] ResendConfirmationDto dto)
        {
            var result = await _authService.ResendConfirmationAsync(dto.Email);
            return Ok(result);
        }

        [AllowAnonymous]
        [EnableRateLimiting("ForgotPasswordPolicy")]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await _authService.ForgotPasswordAsync(dto);
            return Ok("If the email exists, a reset link has been sent.");
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);
            return Ok(result);
        }

        private void SetRefreshCookie(string refreshToken)
        {
            Response.Cookies.Append("gfm_refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/api/auth/refresh",
                MaxAge = TimeSpan.FromDays(7)
            });
        }

        private void ClearRefreshCookie()
        {
            Response.Cookies.Delete("gfm_refresh_token", new CookieOptions
            {
                Path = "/api/auth/refresh"
            });
        }
    }
}
