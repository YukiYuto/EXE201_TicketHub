using System.Security.Claims;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.ChatRoom;

namespace TicketHub.Services.IService;

public interface IChatRoomService
{
    Task<ResponseDto> GetChatRooms
    (
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 0,
        int pageSize = 0
    );

    Task<ResponseDto> GetChatRoom(ClaimsPrincipal user, Guid userId);
    Task<ResponseDto> CreateChatRoom(ClaimsPrincipal user, CreateChatRoomDto createChatRoomDto);
    Task<ResponseDto> UpdateChatRoom(ClaimsPrincipal user, UpdateChatRoomDto updateChatRoomDto);
    Task<ResponseDto> DeleteChatRoom(ClaimsPrincipal user, Guid chatRoomId);
}