using System.Security.Claims;
using Api.DTOs;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IUserService userService, IJwtService jwtService, IConfiguration configuration, INotificationService notificationService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IConfiguration _configuration = configuration;
        private readonly INotificationService _notificationService = notificationService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var user = await _userService.FindByUsernameAndPasswordAsync(loginDTO.Username, loginDTO.Password);
                if (user == null)
                {
                    return Unauthorized("Usuário ou senha incorretos.");
                }

                var token = _jwtService.GenerateJwt(user);

                var expiresInHours = int.Parse(_configuration["Jwt:ExpiresInHours"]);
                var expiresAt = DateTimeOffset.UtcNow.AddHours(expiresInHours);

                Response.Cookies.Append("Token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = expiresAt
                });

                return Ok(new { message = "Login realizado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                Response.Cookies.Delete("Token");
                return Ok(new { message = "Logout realizado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetUserInfo()
        {
            try
            {
                var token = Request.Cookies["Token"];

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("Token de autenticação não encontrado.");
                }

                var claimsPrincipal = _jwtService.ValidateToken(token);

                if (claimsPrincipal?.Identity == null || !claimsPrincipal.Identity.IsAuthenticated)
                {
                    return Unauthorized("Token inválido ou não autenticado.");
                }

                var email = claimsPrincipal.Identity.Name;

                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("Email não encontrado no token.");
                }

                var user = await _userService.FindByEmailAsync(email);

                if (user == null)
                {
                    return NotFound("Usuário não encontrado.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpDelete("notifications/{id}")]
        [Authorize(Roles = "Administrator, Agent, Client")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            try
            {
                await _notificationService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("notifications")]
        [Authorize(Roles = "Administrator, Agent, Client")]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var notifications = await _notificationService.GetByUserIdAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
