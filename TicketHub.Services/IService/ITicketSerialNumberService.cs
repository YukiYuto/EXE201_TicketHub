using System.Security.Claims;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.TicketSerialNumber;

namespace TicketHub.Services.IService;

public interface ITicketSerialNumberService
{
    Task<ResponseDto> CreateTicketSerialNumber(ClaimsPrincipal user,
        List<CreateTicketSerialNumberDto> createTicketSerialNumberDto);

    Task<ResponseDto> GetTicketSerialNumbers
    (
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 1,
        int pageSize = 10,
        Guid? ticketTemplateId = null
    );

    Task<ResponseDto> GetTicketSerialNumberById(ClaimsPrincipal user, Guid serialNumberId);

    Task<ResponseDto> UpdateTicketSerialNumber(ClaimsPrincipal user,
        UpdateTicketSerialNumberDto updateTicketSerialNumberDto);

    Task<ResponseDto> DeleteTicketSerialNumber(ClaimsPrincipal user, Guid serialNumberId);
}