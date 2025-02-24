using System.Security.Claims;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Ticket;

namespace TicketHub.Services.IService;

public interface ITicketService
{
    Task<ResponseDto> GetTickets
    (
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 0,
        int pageSize = 0
    );

    Task<ResponseDto> GetTicket(ClaimsPrincipal user, Guid ticketId);
    Task<ResponseDto> GetTicketByUserId(ClaimsPrincipal user);
    Task<ResponseDto> CreateTicketByCustomer(ClaimsPrincipal user, CreateTicketDto createTicketDtos);
    Task<ResponseDto> CreateTicketByOrganiztion(ClaimsPrincipal user, List<CreateTicketDto> createTicketDtos);
    Task<ResponseDto> UpdateTicket(ClaimsPrincipal user, UpdateTicketDto updateTicketDto);
    Task<ResponseDto> DeleteTicket(ClaimsPrincipal user, Guid ticketId);
    Task<ResponseDto> UploadTicketImage(ClaimsPrincipal user, UploadTicketImgDto uploadTicketImgDto);
    Task<ResponseDto> AcceptTicket(ClaimsPrincipal user, Guid ticketId);
    Task<ResponseDto> RejectTicket(ClaimsPrincipal user, Guid ticketId);
    Task<ResponseDto> GenerateQRCode(Guid ticketId, string serialNumber);
    Task<ResponseDto> ValidateAndUpdateTicket(Guid ticketId, string serialNumber);
}