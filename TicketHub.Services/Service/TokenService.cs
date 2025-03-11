using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Services.IService;

namespace TicketHub.Services.Service;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _jwtSecretKey;
    private readonly IRedisService _redisService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public TokenService
    (
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IRedisService redisService,
        IUnitOfWork unitOfWork
    )
    {
        _userManager = userManager;
        _configuration = configuration;
        _redisService = redisService;
        _unitOfWork = unitOfWork;
        _jwtSecretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? ""));
    }

    public async Task<string> GenerateJwtAccessTokenAsync(ApplicationUser user)
    {
        var userInfo = await _unitOfWork.CustomerRepository.GetWithOrganizerByUserIdAsync(user.Id);
        var customer = userInfo.Customer;
        var organizer = userInfo.Organizer;

        var userRoles = await _userManager.GetRolesAsync(user);

        //Base info
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? ""),
            new(ClaimTypes.Email, user.Email ?? ""),
            new("FullName", user.FullName ?? ""),
            new("PhoneNumber", user.PhoneNumber ?? ""),
            new("Address", user.Address ?? "")
        };

        //Customer info
        if (customer != null)
            authClaims.AddRange(new[]
            {
                new Claim("CCCD", customer.CCCD ?? ""),
                new Claim("Gender", customer.Gender ?? ""),
                new Claim("CustomerId", customer.CustomerId.ToString())
            });
        //Organizer info
        if (organizer != null)
            authClaims.AddRange(new[]
            {
                new Claim("OrganizationName", organizer.OrganizationName ?? ""),
                new Claim("TaxId", organizer.TaxId ?? ""),
                new Claim("OrganizerId", organizer.OrganizerId.ToString())
            });

        // Add roles to claims
        authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Create JWT token
        var tokenObject = new JwtSecurityToken(
            _configuration["JWT:ValidIssuer"],
            _configuration["JWT:ValidAudience"],
            notBefore: DateTime.UtcNow,
            expires: DateTime.Now.AddMinutes(60),
            claims: authClaims,
            signingCredentials: new SigningCredentials(_jwtSecretKey, SecurityAlgorithms.HmacSha256)
        );

        // Create JWT access token
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenObject);

        return accessToken;
    }

    public Task<string> GenerateJwtRefreshTokenAsync(ApplicationUser user)
    {
        // Create a list of claims containing user information
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id)
        };

        // Create JWT token object
        var tokenObject = new JwtSecurityToken(
            _configuration["JWT:ValidIssuer"],
            _configuration["JWT:ValidAudience"],
            notBefore: DateTime.UtcNow,
            expires: DateTime.Now.AddDays(1),
            claims: authClaims,
            signingCredentials: new SigningCredentials(_jwtSecretKey, SecurityAlgorithms.HmacSha256)
        );

        // Token generation successful
        var refreshToken = new JwtSecurityTokenHandler().WriteToken(tokenObject);

        return Task.FromResult(refreshToken);
    }

    public async Task<bool> StoreRefreshToken(string userId, string refreshToken)
    {
        var redisKey = $"userId:{userId}:refreshToken";
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
        }, out var validatedToken);

        return principal;
    }

    public async Task<string?> RetrieveRefreshToken(string userId)
    {
        var redisKey = $"userId:{userId}:refreshToken";
        var refreshToken = await _redisService.RetrieveString(redisKey);

        return string.IsNullOrEmpty(refreshToken) ? null : refreshToken;
    }

    public async Task<bool> DeleteRefreshToken(string userId)
    {
        var redisKey = $"userId:{userId}:refreshToken";
        return await _redisService.DeleteString(redisKey);
    }
}