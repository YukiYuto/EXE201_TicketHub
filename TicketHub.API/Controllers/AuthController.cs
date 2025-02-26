using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO.Authentication;
using TicketHub.Models.DTO.Email;
using TicketHub.Services.IService;

namespace TicketHub.API.Controllers;

public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthService _authService;

    public AuthController(UserManager<ApplicationUser> userManager, IAuthService authService)
    {
        _userManager = userManager;
        _authService = authService;
    }

    [HttpPost("sign-up-customer")]
    public async Task<IActionResult> SignUpCustomer([FromBody] SignUpCustomerDto signUpCustomerDto)
    {
        var response = await _authService.SignUpCustomer(signUpCustomerDto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("sign-up-organization")]
    public async Task<IActionResult> SignUpOrganization([FromBody] SignUpOrganizationDto signUpOrganizationDto)
    {
        var response = await _authService.SignUpOrganization(signUpOrganizationDto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] SignInDto signInDto)
    {
        var response = await _authService.SignIn(signInDto);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPost("update-user-profile")]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileDto updateUserProfileDto)
    {
        var response = await _authService.UpdateUserProfile(User, updateUserProfileDto);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("user")]
    [Authorize]
    public async Task<IActionResult> FetchUserByToken()
    {
        var responseDto = await _authService.FetchUserByToken(User);
        return StatusCode(responseDto.StatusCode, responseDto);
    }
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        var responseDto = await _authService.RefreshToken(refreshTokenDto);
        return StatusCode(responseDto.StatusCode, responseDto);
    }
    
    [HttpPost("email/verification/send")]
    public async Task<IActionResult> SendVerifyEmail([FromBody] EmailDto emailDto)
    {
        var responseDto = await _authService.SendVerifyEmail(emailDto);
        return StatusCode(responseDto.StatusCode, responseDto);
    }
    
    [HttpPost("email/verification/verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto verifyEmailDto)
    {
        var responseDto = await _authService.VerifyEmail(verifyEmailDto);
        return StatusCode(responseDto.StatusCode, responseDto);
    }
    
    [HttpPost("password/change")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        var responseDto = await _authService.ChangePassword(User, changePasswordDto);
        return StatusCode(responseDto.StatusCode, responseDto);
    }
    
    [HttpPost("password/forgot")]
    public async Task<IActionResult> ForgotPassword([FromBody] EmailDto emailDto)
    {
        var responseDto = await _authService.ForgotPassword(emailDto);
        return StatusCode(responseDto.StatusCode, responseDto);
    }
    
    [HttpPost("password/reset")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        var responseDto = await _authService.ResetPassword(resetPasswordDto);
        return StatusCode(responseDto.StatusCode, responseDto);
    }
}