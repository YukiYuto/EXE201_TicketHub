using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketHub.Models.DTO;
using TicketHub.Services.IService;
using TicketHub.Utility.Constants;

namespace TicketHub.API.Controllers;

[Route("api/revenue")]
[ApiController]
public class ManagerController : ControllerBase
{
    private readonly IManagerService _managerService;

    public ManagerController(IManagerService managerService)
    {
        _managerService = managerService;
    }

    [HttpGet("profit")]
    [Authorize(Roles = StaticUserRoles.Manager)]
    public async Task<ActionResult<ResponseDto>> GetRevenue(
        DateTime startDate,
        DateTime endDate,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        var responseDto =
            await _managerService.GetRevenueProfit(startDate, endDate, pageNumber, pageSize);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpGet("customer")]
    [Authorize(Roles = StaticUserRoles.Manager)]
    public async Task<ActionResult<ResponseDto>> GetCustomerRevenue(
        [FromQuery] int pageNumber = 1,
        int pageSize = 10,
        string? filterOn = null,
        string? filterQuery = null,
        string? sortBy = null
    )
    {
        var responseDto =
            await _managerService.GetAllCustomer(User, pageNumber, pageSize, filterOn, filterQuery, sortBy);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpGet("organizer")]
    [Authorize(Roles = StaticUserRoles.Manager)]
    public async Task<ActionResult<ResponseDto>> GetOrganizerRevenue(
        [FromQuery] int pageNumber = 1,
        int pageSize = 10,
        string? filterOn = null,
        string? filterQuery = null,
        string? sortBy = null
    )
    {
        var responseDto =
            await _managerService.GetAllOrganizer(User, pageNumber, pageSize, filterOn, filterQuery, sortBy);
        return StatusCode(responseDto.StatusCode, responseDto);
    }
}