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

    Task<ResponseDto> GetTicketTemplates(
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 0,
        int pageSize = 0
    );

    Task<ResponseDto> GetTicketByUserId(ClaimsPrincipal user);
    Task<ResponseDto> GetTicketTemplateByUserId(ClaimsPrincipal user);
    Task<ResponseDto> GetTicketTemplateById(Guid ticketTemplateId);
    Task<ResponseDto> GetTicketTemplateByEventId(Guid eventId);
    Task<ResponseDto> UpdateTicketTemplate(ClaimsPrincipal user, UpdateTicketDto updateTicketDto);
    Task<ResponseDto> GenerateQrCode(Guid ticketId, Guid serialNumber);
    Task<ResponseDto> ValidateAndUpdateTicket(Guid ticketId, Guid serialNumber);
}