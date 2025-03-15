using System.Security.Claims;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Ticket;
using TicketHub.Models.DTO.TicketTemplate;

namespace TicketHub.Services.IService;

public interface ITicketService
{
    //Task<ResponseDto> CreateTicketByCustomer(ClaimsPrincipal user, CreateTicketDto createTicketDtos);
    Task<ResponseDto> CreateTicketByOrganization(ClaimsPrincipal user,
        CreateTicketTemplateDto createTicketTemplateDto);

    Task<ResponseDto> GetTicketTemplates
    (
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 0,
        int pageSize = 0
    );

    Task<ResponseDto> GetTicketByUserId(ClaimsPrincipal user);

    Task<ResponseDto> GetTicketTemplateById(Guid ticketTemplateId);
    Task<ResponseDto> GetTicketTemplateByEventId(ClaimsPrincipal user, Guid eventId);

    Task<ResponseDto> UpdateTicketTemplate(ClaimsPrincipal user, UpdateTicketDto updateTicketDto);

    /*
    Task<ResponseDto> CreateTicketByCustomer(ClaimsPrincipal user, CreateTicketDto createTicketDtos);
    Task<ResponseDto> DeleteTicket(ClaimsPrincipal user, Guid ticketId);
    Task<ResponseDto> AcceptTicket(ClaimsPrincipal user, Guid ticketId);
    Task<ResponseDto> RejectTicket(ClaimsPrincipal user, Guid ticketId);
    Task<ResponseDto> GenerateQRCode(Guid ticketId, string serialNumber);
    Task<ResponseDto> ValidateAndUpdateTicket(Guid ticketId, string serialNumber);*/
}