using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Event;
using TicketHub.Services.IService;

namespace TicketHub.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> GetEvents
    (
        [FromQuery] string? filterOn,
        [FromQuery] string? filterQuery,
        [FromQuery] string? sortBy,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var responseDto = await _eventService.GetEvents(User, filterOn, filterQuery, sortBy, pageNumber, pageSize);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpGet("{eventId}")]
    public async Task<ActionResult<ResponseDto>> GetEvent
    (
        [FromRoute] Guid eventId
    )
    {
        var responseDto = await _eventService.GetEvent(User, eventId);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ResponseDto>> CreateEvent
    (
        [FromBody] CreateEventDto createLocationDto
    )
    {
        var responseDto = await _eventService.CreateEvent(User, createLocationDto);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult<ResponseDto>> UpdateEvent
    (
        [FromBody] UpdateEventDto updateLocationDto
    )
    {
        var responseDto = await _eventService.UpdateEvent(User, updateLocationDto);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpDelete("{eventId}")]
    [Authorize]
    public async Task<ActionResult<ResponseDto>> DeleteEvent
    (
        [FromRoute] Guid eventId
    )
    {
        var responseDto = await _eventService.DeleteEvent(User, eventId);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpGet("search")]
    public async Task<ActionResult<ResponseDto>> SearchEvents
    (
        [FromQuery] string eventName
    )
    {
        var responseDto = await _eventService.SearchEvent(User, eventName);
        return StatusCode(responseDto.StatusCode, responseDto);
    }
}