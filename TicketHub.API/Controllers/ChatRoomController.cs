using Microsoft.AspNetCore.Mvc;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.ChatRoom;
using TicketHub.Services.IService;

namespace TicketHub.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatRoomController : ControllerBase
{
    private readonly IChatRoomService _chatRoomService;

    public ChatRoomController(IChatRoomService chatRoomService)
    {
        _chatRoomService = chatRoomService;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> GetChatRooms
    (
        [FromQuery] string? filterOn,
        [FromQuery] string? filterQuery,
        [FromQuery] string? sortBy,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var responseDto = await _chatRoomService.GetChatRooms(User, filterOn, filterQuery, sortBy, pageNumber, pageSize);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<ResponseDto>> ChatRoom
    (
        [FromRoute] Guid userId
    )
    {
        var responseDto = await _chatRoomService.GetChatRoom(User, userId);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpPost]
    public async Task<ActionResult<ResponseDto>> CreateChatRoom
    (
        [FromBody] CreateChatRoomDto createChatRoomDto
    )
    {
        var responseDto = await _chatRoomService.CreateChatRoom(User, createChatRoomDto);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpPut]
    public async Task<ActionResult<ResponseDto>> UpdateChatRoom
    (
        [FromBody] UpdateChatRoomDto updateChatRoomDto
    )
    {
        var responseDto = await _chatRoomService.UpdateChatRoom(User, updateChatRoomDto);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpDelete("{chatRoomId}")]
    public async Task<ActionResult<ResponseDto>> DeleteChatRoom
    (
        [FromRoute] Guid chatRoomId
    )
    {
        var responseDto = await _chatRoomService.DeleteChatRoom(User, chatRoomId);
        return StatusCode(responseDto.StatusCode, responseDto);
    }
}