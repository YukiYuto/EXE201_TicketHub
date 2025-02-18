using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TicketHub.Models.Domain;
using TicketHub.Services.IService;

namespace TicketHub.Services.Service;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IRedisService _redisService;

    public TokenService(UserManager<ApplicationUser> userManager, IConfiguration configuration,
        IRedisService redisService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _redisService = redisService;
    }

    public async Task<string> GenerateJwtAccessTokenCustomerAsync(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("FullName", user.FullName),
            new Claim("PhoneNumber", user.PhoneNumber),
            new Claim("Address", user.Address),
            new Claim("CCCD", user.CCCD)
        };

        // Thêm role của người dùng vào claims
        foreach (var role in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Tạo security key và signing credentials
        var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? string.Empty));
        var signingCredentials = new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256);

        // Tạo đối tượng JWT token
        var tokenObject = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(5),
            claims: authClaims,
            signingCredentials: signingCredentials
        );

        // Tạo JWT access token
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenObject);

        return accessToken;
    }

    public async Task<string> GenerateJwtAccessTokenOrganizationAsync(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("OrganizationName", user.OrganizationName),
            new Claim("TaxId", user.TaxId),
            new Claim("PhoneNumber", user.PhoneNumber),
            new Claim("Address", user.Address)
        };

        // Thêm role của người dùng vào claims
        foreach (var role in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Tạo security key và signing credentials
        var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? string.Empty));
        var signingCredentials = new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256);

        // Tạo đối tượng JWT token
        var tokenObject = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(5),
            claims: authClaims,
            signingCredentials: signingCredentials
        );

        // Tạo JWT access token
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenObject);

        return accessToken;
    }

    public Task<string> GenerateJwtRefreshTokenAsync(ApplicationUser user)
    {
        // Create a list of claims containing user information
        var authClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

        // Create cryptographic objects for tokens
        var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        var signingCredentials = new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256);

        // Create JWT token object
        var tokenObject = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddDays(1), //Expiration time is 1 day
            claims: authClaims,
            signingCredentials: signingCredentials
        );

        // Token generation successful
        var refreshToken = new JwtSecurityTokenHandler().WriteToken(tokenObject);

        return Task.FromResult(refreshToken);
    }

    public async Task<bool> StoreRefreshToken(string userId, string refreshToken)
    {
        string redisKey = $"userId:{userId}:refreshToken";
        var result = await _redisService.StoreString(redisKey, refreshToken, TimeSpan.FromDays(1)); 
        return result;
    }

    public async Task<ClaimsPrincipal> GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? string.Empty);
        var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["JWT:ValidIssuer"],
            ValidAudience = _configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        }, out SecurityToken validatedToken);

        return principal;
    }
}