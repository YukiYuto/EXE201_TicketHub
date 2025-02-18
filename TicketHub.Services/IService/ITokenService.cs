using System.Security.Claims;
using TicketHub.Models.Domain;

namespace TicketHub.Services.IService;

public interface ITokenService
{
    Task<string> GenerateJwtAccessTokenCustomerAsync(ApplicationUser user);
    Task<string> GenerateJwtAccessTokenOrganizationAsync(ApplicationUser user);
    Task<string> GenerateJwtRefreshTokenAsync(ApplicationUser user);
    Task<bool> StoreRefreshToken(string userId, string refreshToken);
    Task<ClaimsPrincipal> GetPrincipalFromToken(string token);
}