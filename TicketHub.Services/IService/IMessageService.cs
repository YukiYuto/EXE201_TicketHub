using System.Security.Claims;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Message;

namespace TicketHub.Services.IService;

public interface IMessageService
{
    Task<ResponseDto> GetMessages
    (
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 0,
        int pageSize = 0
    );

    Task<ResponseDto> GetMessage(ClaimsPrincipal user, Guid messageId);

    Task<ResponseDto> CreateMessage(ClaimsPrincipal user, CreateMessageDto createMessageDto);

    Task<ResponseDto> UpdateMessage(ClaimsPrincipal user, UpdateMessageDto updateMessageDto);

    Task<ResponseDto> DeleteMessage(ClaimsPrincipal user, Guid messageId);
}