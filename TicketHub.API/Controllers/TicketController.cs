using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Ticket;
using TicketHub.Models.DTO.TicketTemplate;
using TicketHub.Services.IService;
using TicketHub.Utility.Constants;

namespace TicketHub.API.Controllers;

[Route("api/tickets")]
[ApiController]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpPost("create-ticket-template")]
    [Authorize(Roles = StaticUserRoles.Organization)]
    public async Task<ActionResult<ResponseDto>> CreateTicketTemplate(
        [FromBody] CreateTicketTemplateDto createTicketTemplateDto)
    {
        var responseDto = await _ticketService.CreateTicketByOrganization(User, createTicketTemplateDto);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpGet("templates")]
    public async Task<ActionResult<ResponseDto>> GetTicketTemplates(
        [FromQuery] string? filterOn,
        [FromQuery] string? filterQuery,
        [FromQuery] string? sortBy,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var responseDto = await _ticketService.GetTicketTemplates(
            User, filterOn, filterQuery, sortBy, pageNumber, pageSize
        );
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpGet("user")]
    [Authorize]
    public async Task<ActionResult<ResponseDto>> GetTicketByUserId()
    {
        var responseDto = await _ticketService.GetTicketByUserId(User);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpGet("template/{ticketTemplateId}")]
    public async Task<ActionResult<ResponseDto>> GetTicketTemplateById(Guid ticketTemplateId)
    {
        var responseDto = await _ticketService.GetTicketTemplateById(ticketTemplateId);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpGet("event/{eventId}")]
    public async Task<ActionResult<ResponseDto>> GetTicketTemplateByEventId(Guid eventId)
    {
        var responseDto = await _ticketService.GetTicketTemplateByEventId(User, eventId);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpPut("update-ticket-template")]
    [Authorize(Roles = StaticUserRoles.Organization)]
    public async Task<ActionResult<ResponseDto>> UpdateTicketTemplate(
        [FromBody] UpdateTicketDto updateTicketTemplateDto)
    {
        var responseDto = await _ticketService.UpdateTicketTemplate(User, updateTicketTemplateDto);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

/*
    [HttpGet("generate-qr-code")]
    public async Task<ActionResult<ResponseDto>> GenerateQRCode(Guid ticketId, string serialNumber)
    {
        var responseDto = await _ticketService.GenerateQRCode(ticketId, serialNumber);

        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpPost("scan-qr-code")]
    public async Task<ActionResult<ResponseDto>> ScanQRCode(Guid ticketId, string serialNumber)
    {
        if (ticketId == null || ticketId == Guid.Empty || string.IsNullOrEmpty(serialNumber))
            return BadRequest(new { message = "Dữ liệu không hợp lệ." });

        var responseDto = await _ticketService.ValidateAndUpdateTicket(ticketId, serialNumber);

        return StatusCode(responseDto.StatusCode, responseDto);
    }*/
}