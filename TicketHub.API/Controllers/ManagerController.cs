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

    [HttpGet("revenue")]
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
}