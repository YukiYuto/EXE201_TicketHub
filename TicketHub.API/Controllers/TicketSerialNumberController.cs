using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketHub.Models.DTO.TicketSerialNumber;
using TicketHub.Services.IService;
using TicketHub.Utility.Constants;

namespace TicketHub.API.Controllers;

[Route("api/ticket-serial-number")]
[ApiController]
public class TicketSerialNumberController : ControllerBase
{
    private readonly ITicketSerialNumberService _ticketSerialNumberService;

    public TicketSerialNumberController(ITicketSerialNumberService ticketSerialNumberService)
    {
        _ticketSerialNumberService = ticketSerialNumberService;
    }

    [HttpPost]
    [Authorize(Roles = StaticUserRoles.Organization)]
    public async Task<IActionResult> CreateTicketSerialNumber(
        [FromBody] List<CreateTicketSerialNumberDto> createTicketSerialNumberDto)
    {
        var responseDto = await _ticketSerialNumberService.CreateTicketSerialNumber(User, createTicketSerialNumberDto);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetTicketSerialNumbers(
        [FromQuery] string? filterOn,
        [FromQuery] string? filterQuery,
        [FromQuery] string? sortBy,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var responseDto =
            await _ticketSerialNumberService.GetTicketSerialNumbers
            (
                User,
                filterOn,
                filterQuery,
                sortBy,
                pageNumber,
                pageSize
            );
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpGet("{serialNumberId}")]
    [Authorize]
    public async Task<IActionResult> GetTicketSerialNumberById([FromHeader] Guid serialNumberId)
    {
        var responseDto = await _ticketSerialNumberService.GetTicketSerialNumberById(User, serialNumberId);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateTicketSerialNumber(
        [FromBody] UpdateTicketSerialNumberDto updateTicketSerialNumberDto)
    {
        var responseDto = await _ticketSerialNumberService.UpdateTicketSerialNumber(User, updateTicketSerialNumberDto);
        return StatusCode(responseDto.StatusCode, responseDto);
    }
}