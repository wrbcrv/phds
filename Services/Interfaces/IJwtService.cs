using System.Security.Claims;
using Api.DTOs;

namespace Api.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwt(UserResponseDTO user);
        ClaimsPrincipal ValidateToken(string token);
    }
}
