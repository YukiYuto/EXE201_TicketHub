using System.Security.Claims;
using AutoMapper;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.ChatRoom;
using TicketHub.Services.IService;

namespace TicketHub.Services.Service;

public class ChatRoomService : IChatRoomService
{
    private readonly IUnitOfWork _unitOfWork;
    private IMapper _mapper;

    public ChatRoomService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseDto> GetChatRooms
    (
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        IEnumerable<ChatRoom> allChatRooms = null!;

        allChatRooms = await _unitOfWork.ChatRoomRepository.GetAllAsync();

        if (!allChatRooms.Any())
        {
            return new ResponseDto()
            {
                Message = "There are no chat rooms",
                IsSuccess = true,
                StatusCode = 404,
                Result = null
            };
        }

        var listChatRooms = allChatRooms.ToList();

        // Filter Query
        if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
        {
            switch (filterOn.Trim().ToLower())
            {
                case "roomname":
                    listChatRooms = listChatRooms.Where(x =>
                        x.NameRoom.Contains(filterQuery, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    break;
                default:
                    break;
            }
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            var sortParams = sortBy.Trim().ToLower().Split('_');
            var sortField = sortParams[0];
            var sortDirection = sortParams.Length > 1 ? sortParams[1] : "asc";

            switch (sortField)
            {
                case "roomname":
                    listChatRooms = sortDirection == "desc"
                        ? listChatRooms.OrderByDescending(x => x.NameRoom).ToList()
                        : listChatRooms.OrderBy(x => x.NameRoom).ToList();
                    break;

                default:
                    listChatRooms = listChatRooms.OrderBy(x => x.CreateTime).ToList();
                    break;
            }
        }
        else
        {
            listChatRooms = listChatRooms.OrderBy(x => x.CreateTime).ToList();
        }

        if (pageNumber > 0 && pageSize > 0)
        {
            var skipResult = (pageNumber - 1) * pageSize;
            listChatRooms = listChatRooms.Skip(skipResult).Take(pageSize).ToList();
        }

        var chatRoomDto = listChatRooms.Select(chatRoomItem => new GetChatRoomDto()
        {
            ChatRoomId = chatRoomItem.ChatRoomId,
            NameRoom = chatRoomItem.NameRoom,
            SendMessageUserId = chatRoomItem.SendMessageUserId,
            ReceiveMessageUserId = chatRoomItem.ReceiveMessageUserId,
            CreateTime = chatRoomItem.CreateTime,
            UpdateTime = chatRoomItem.UpdateTime,
        }).ToList();

        return new ResponseDto()
        {
            Message = "Get chat rooms successfully",
            IsSuccess = true,
            StatusCode = 200,
            Result = chatRoomDto
        };
    }

    public async Task<ResponseDto> GetChatRoom(ClaimsPrincipal user, Guid userId)
    {
        var messages =
            await _unitOfWork.MessageRepository.GetAllAsync(x =>
                x.SendMessageUserId == userId || x.ReceiveMessageUserId == userId);
        if (messages == null || !messages.Any())
        {
            return new ResponseDto
            {
                Message = "No messages found for the user",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }

        var chatRoomIds = messages.Select(x => x.ChatRoomId).Distinct().ToList();
        var chatRooms = await _unitOfWork.ChatRoomRepository.GetAllAsync(x => chatRoomIds.Contains(x.ChatRoomId));
        if (chatRooms == null || !chatRooms.Any())
        {
            return new ResponseDto
            {
                Message = "No chat rooms found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }

        var chatRoomDtos = chatRooms.Select(x => _mapper.Map<GetChatRoomDto>(x)).ToList();

        return new ResponseDto
        {
            Message = "Chat rooms found successfully",
            Result = chatRoomDtos,
            IsSuccess = true,
            StatusCode = 200
        };
    }

    public async Task<ResponseDto> CreateChatRoom(ClaimsPrincipal user, CreateChatRoomDto createChatRoomDto)
    {
        ChatRoom newChatRoom = new ChatRoom()
        {
            ChatRoomId = new Guid(),
            NameRoom = createChatRoomDto.NameRoom,
            SendMessageUserId = createChatRoomDto.SendMessageUserId,
            ReceiveMessageUserId = createChatRoomDto.ReceiveMessageUserId,
            CreateTime = createChatRoomDto.CreateTime,
            UpdateTime = createChatRoomDto.UpdateTime,
        };

        await _unitOfWork.ChatRoomRepository.AddAsync(newChatRoom);
        await _unitOfWork.SaveAsync();

        return new ResponseDto
        {
            Message = "Chat room created successfully",
            Result = newChatRoom,
            IsSuccess = true,
            StatusCode = 201
        };
    }

    public async Task<ResponseDto> UpdateChatRoom(ClaimsPrincipal user, UpdateChatRoomDto updateChatRoomDto)
    {
        var chatRoomId =
            await _unitOfWork.ChatRoomRepository.GetAsync(x => x.ChatRoomId == updateChatRoomDto.ChatRoomId);
        if (chatRoomId == null)
        {
            return new ResponseDto
            {
                Message = "Chat room not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }

        chatRoomId.ChatRoomId = updateChatRoomDto.ChatRoomId;
        chatRoomId.NameRoom = updateChatRoomDto.NameRoom;
        chatRoomId.SendMessageUserId = updateChatRoomDto.SendMessageUserId;
        chatRoomId.ReceiveMessageUserId = updateChatRoomDto.ReceiveMessageUserId;
        chatRoomId.CreateTime = updateChatRoomDto.CreateTime;
        chatRoomId.UpdateTime = updateChatRoomDto.UpdateTime;

        _unitOfWork.ChatRoomRepository.Update(chatRoomId);
        var save = await _unitOfWork.SaveAsync();

        return new ResponseDto
        {
            Message = "Chat room updated successfully",
            Result = updateChatRoomDto,
            IsSuccess = true,
            StatusCode = 201
        };
    }

    public async Task<ResponseDto> DeleteChatRoom(ClaimsPrincipal user, Guid chatRoomId)
    {
        var chatRooms = await _unitOfWork.ChatRoomRepository.GetAsync(x => x.ChatRoomId == chatRoomId);
        if (chatRooms == null)
        {
            return new ResponseDto
            {
                Message = "Chat room not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }

        _unitOfWork.ChatRoomRepository.Remove(chatRooms);
        var save = await _unitOfWork.SaveAsync();

        return new ResponseDto()
        {
            Message = "Chat rooms delete successfully",
            Result = chatRooms,
            IsSuccess = true,
            StatusCode = 201
        };
    }
}