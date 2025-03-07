using System.Security.Claims;
using AutoMapper;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Message;
using TicketHub.Services.IService;

namespace TicketHub.Services.Service;

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;
private IMapper _mapper;

public MessageService(IUnitOfWork unitOfWork, IMapper mapper)
{
    _unitOfWork = unitOfWork;
    _mapper = mapper;
}

public async Task<ResponseDto> CreateMessage(ClaimsPrincipal user, CreateMessageDto createMessageDto)
{
    Message newMessage = new Message()
    {
        MessageId = new Guid(),
        MessageContent = createMessageDto.MessageContent,
        SendMessageUserId = createMessageDto.SendMessageUserId,
        ReceiveMessageUserId = createMessageDto.ReceiveMessageUserId,
        CreateTime = DateTime.Now,
        ChatRoomId = createMessageDto.ChatRoomId
    };

    await _unitOfWork.MessageRepository.AddAsync(newMessage);
    await _unitOfWork.SaveAsync();

    return new ResponseDto
    {
        Message = "Message created successfully",
        Result = newMessage,
        IsSuccess = true,
        StatusCode = 201
    };
}

public async Task<ResponseDto> DeleteMessage(ClaimsPrincipal user, Guid messageId)
{
    var message = await _unitOfWork.MessageRepository.GetAsync(x => x.MessageId == messageId);
    if (message == null)
    {
        return new ResponseDto
        {
            Message = "Message not found",
            IsSuccess = false,
            StatusCode = 404
        };
    }

    _unitOfWork.MessageRepository.Remove(message);
    await _unitOfWork.SaveAsync();

    return new ResponseDto
    {
        Message = "Message deleted successfully",
        IsSuccess = true,
        StatusCode = 200
    };
}

public async Task<ResponseDto> GetMessage(ClaimsPrincipal user, Guid chatRoomId)
{
    var messages = await _unitOfWork.MessageRepository.GetAllAsync(x => x.ChatRoomId == chatRoomId);
    if (messages == null || !messages.Any())
    {
        return new ResponseDto
        {
            Message = "No messages found for the chat room",
            Result = null,
            IsSuccess = false,
            StatusCode = 404
        };
    }

    var messageDtos = messages.Select(x => _mapper.Map<GetMessageDto>(x)).ToList();

    return new ResponseDto
    {
        Message = "Messages found successfully",
        Result = messageDtos,
        IsSuccess = true,
        StatusCode = 200
    };
}


public async Task<ResponseDto> GetMessages(ClaimsPrincipal user, string? filterOn, string? filterQuery, string? sortBy, int pageNumber = 0, int pageSize = 0)
{
    var allMessages = await _unitOfWork.MessageRepository.GetAllAsync();

    if (!allMessages.Any())
    {
        return new ResponseDto
        {
            Message = "There are no messages",
            IsSuccess = true,
            StatusCode = 404,
            Result = null
        };
    }

    var listMessages = allMessages.ToList();

    // Filter
    if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
    {
        switch (filterOn.Trim().ToLower())
        {
            case "MessageContent":
                listMessages = listMessages
                    .Where(x => x.MessageContent.Contains(filterQuery, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
                break;
            default:
                break;
        }
    }

    // Sort
    if (!string.IsNullOrEmpty(sortBy))
    {
        var sortParams = sortBy.Trim().ToLower().Split('_');
        var sortField = sortParams[0];
        var sortDirection = sortParams.Length > 1 ? sortParams[1] : "asc";

        switch (sortField)
        {
            case "createtime":
                listMessages = sortDirection == "desc"
                    ? listMessages.OrderByDescending(x => x.CreateTime).ToList()
                    : listMessages.OrderBy(x => x.CreateTime).ToList();
                break;
            default:
                break;
        }
    }
    else
    {
        listMessages = listMessages.OrderBy(x => x.CreateTime).ToList();
    }

    // Phân trang
    var skipResult = (pageNumber - 1) * pageSize;
    listMessages = listMessages.Skip(skipResult).Take(pageSize).ToList();

    // Map to DTO
    var messageDtos = listMessages.Select(msg => _mapper.Map<GetMessageDto>(msg)).ToList();

    return new ResponseDto
    {
        Message = "Get messages successfully",
        IsSuccess = true,
        StatusCode = 200,
        Result = messageDtos
    };
}

public async Task<ResponseDto> UpdateMessage(ClaimsPrincipal user, UpdateMessageDto updateMessageDto)
{
    var message = await _unitOfWork.MessageRepository.GetAsync(x => x.MessageId == updateMessageDto.MessageId);
    if (message == null)
    {
        return new ResponseDto
        {
            Message = "Message not found",
            IsSuccess = false,
            StatusCode = 404
        };
    }
    
    message.MessageId = updateMessageDto.MessageId;
    message.MessageContent = updateMessageDto.MessageContent;
    message.SendMessageUserId = updateMessageDto.SendMessageUserId;
    message.ReceiveMessageUserId = updateMessageDto.ReceiveMessageUserId;
    message.CreateTime = DateTime.Now;
    message.ChatRoomId = updateMessageDto.ChatRoomId;

    _unitOfWork.MessageRepository.Update(message);
    await _unitOfWork.SaveAsync();

    return new ResponseDto
    {
        Message = "Message updated successfully",
        IsSuccess = true,
        StatusCode = 200,
        Result = message
    };
}
}