using System.Security.Claims;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Event;

namespace TicketHub.Services.IService;

public interface IEventService
{
    Task<ResponseDto> GetEvents
    (
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 0,
        int pageSize = 0
    );

    Task<ResponseDto> GetEvent(ClaimsPrincipal user, Guid eventId);
    Task<ResponseDto> GetEventByUserId(ClaimsPrincipal user);
    Task<ResponseDto> CreateEvent(ClaimsPrincipal user, CreateEventDto createEventDto);
    Task<ResponseDto> UpdateEvent(ClaimsPrincipal user, UpdateEventDto updateEventDto);
    Task<ResponseDto> DeleteEvent(ClaimsPrincipal user, Guid eventId);
    Task<ResponseDto> SearchEvent(ClaimsPrincipal user, string eventName);
}