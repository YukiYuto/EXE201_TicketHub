using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO.Authentication;
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
    public async Task<IActionResult> SignUpCustomer( [FromBody]SignUpCustomerDto signUpCustomerDto)
    {
        var response = await _authService.SignUpCustomer(signUpCustomerDto);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPost("sign-up-organization")]
    public async Task<IActionResult> SignUpOrganization( [FromBody]SignUpOrganizationDto signUpOrganizationDto)
    {
        var response = await _authService.SignUpOrganization(signUpOrganizationDto);
        return StatusCode(response.StatusCode, response);
    }
}