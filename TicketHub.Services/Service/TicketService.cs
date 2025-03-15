using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Ticket;
using TicketHub.Models.DTO.TicketTemplate;
using TicketHub.Services.IService;

namespace TicketHub.Services.Service;

public class TicketService : ITicketService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;


    public TicketService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseDto> CreateTicketByOrganization(ClaimsPrincipal user,
        CreateTicketTemplateDto createTicketTemplateDto)
    {
        try
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return new ResponseDto
                {
                    Message = "User not found",
                    IsSuccess = false,
                    StatusCode = 404
                };

            var organizer = await _unitOfWork.OrganizationRepository.GetAsync(o => o.UserId == userId);
            if (organizer == null)
                return new ResponseDto
                {
                    Message = "You are not the organizer",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 404
                };

            var ticketTemplates = createTicketTemplateDto.TicketTemplates
                .Select(dto => new TicketTemplate
                {
                    TicketTemplateId = Guid.NewGuid(),
                    EventId = dto.EventId,
                    TicketName = dto.TicketName,
                    TicketPrice = dto.TicketPrice,
                    TotalQuantity = dto.TotalQuantity,
                    AvailableQuantity = dto.TotalQuantity,
                    ImageTicket = dto.ImageTicket,
                    Rank = dto.Rank,
                    IsValid = true
                }).ToList();


            await _unitOfWork.TicketTemplateRepository.AddRangeAsync(ticketTemplates);
            await _unitOfWork.SaveAsync();

            return new ResponseDto
            {
                Message = "Tickets created successfully",
                IsSuccess = true,
                StatusCode = 201,
                Result = ticketTemplates
            };
        }
        catch (Exception e)
        {
            return new ResponseDto
            {
                Message = e.Message,
                Result = null,
                IsSuccess = false,
                StatusCode = 400
            };
        }
    }

    public async Task<ResponseDto> GetTicketTemplates
    (
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 0,
        int pageSize = 0
    )
    {
        var (ticketTemplates, totalItems) = await _unitOfWork.TicketTemplateRepository
            .GetFilteredTicketTemplatesAsync(filterOn, filterQuery, sortBy, pageNumber, pageSize);

        var result = ticketTemplates.Select(t => new GetTicketTemplateDto
        {
            TicketTemplateId = t.TicketTemplateId,
            TicketName = t.TicketName,
            EventId = t.EventId,
            TicketPrice = t.TicketPrice,
            TotalQuantity = t.TotalQuantity,
            AvailableQuantity = t.AvailableQuantity,
            ImageTicket = t.ImageTicket,
            Rank = t.Rank
        }).ToList();

        return new ResponseDto
        {
            Message = "Fetched ticket templates successfully",
            IsSuccess = true,
            StatusCode = 200,
            Result = new
            {
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = result
            }
        };
    }

    public async Task<ResponseDto> GetTicketByUserId(ClaimsPrincipal user)
    {
        var userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return new ResponseDto
            {
                Message = "User not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };

        var customer = await _unitOfWork.CustomerRepository.GetAsync(c => c.UserId == userId);
        if (customer == null)
            return new ResponseDto
            {
                Message = "Customer not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };

        // Truy vấn tất cả vé của người dùng từ cơ sở dữ liệu
        /*var tickets = await _unitOfWork.TicketRepository.GetAllAsync(
            x => x.CustomerId == customer.CustomerId && x.IsVisible,
            "TicketTemplate," +
            "TicketTemplate.Event," +
            "TicketTemplate.Event.Category," +
            "TicketSerialNumber"
        );*/


        var tickets = await _unitOfWork.TicketRepository.GetAllAsync(
            x => x.CustomerId == customer.CustomerId && x.IsVisible,
            ticket => ticket.Include(t => t.TicketTemplate)
                .ThenInclude(tt => tt.Event)
                .ThenInclude(e => e.Category)
                .Include(t => t.TicketSerialNumber));


        if (!tickets.Any())
            return new ResponseDto
            {
                Message = "No tickets found for the user",
                Result = tickets,
                IsSuccess = true,
                StatusCode = 200
            };

        // Ánh xạ vé lấy được từ cơ sở dữ liệu thành DTO
        var ticketDtos = _mapper.Map<List<GetTicketDto>>(tickets);

        return new ResponseDto
        {
            Message = "Get Ticket successfully",
            Result = ticketDtos,
            IsSuccess = true,
            StatusCode = 200
        };
    }

    public async Task<ResponseDto> GetTicketTemplateByEventId(ClaimsPrincipal user, Guid eventId)
    {
        var events = await _unitOfWork.TicketTemplateRepository.GetAllAsync(e => e.EventId == eventId);

        var eventDto = _mapper.Map<List<GetTicketTemplateDto>>(events);
        if (eventDto == null)
            return new ResponseDto
            {
                Message = "Event not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };

        return new ResponseDto
        {
            Message = "Get TicketTemplate successfully",
            Result = eventDto,
            IsSuccess = true,
            StatusCode = 200
        };
    }

    public async Task<ResponseDto> GetTicketTemplateById(Guid ticketTemplateId)
    {
        var ticketTemplate =
            await _unitOfWork.TicketTemplateRepository.GetAsync(t => t.TicketTemplateId == ticketTemplateId);

        if (ticketTemplate == null)
            return new ResponseDto
            {
                Message = "TicketTemplate not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };

        var ticketTemplateDto = _mapper.Map<GetTicketTemplateDto>(ticketTemplate);

        return new ResponseDto
        {
            Message = "Get TicketTemplate successfully",
            Result = ticketTemplateDto,
            IsSuccess = true,
            StatusCode = 200
        };
    }


    public async Task<ResponseDto> UpdateTicketTemplate(ClaimsPrincipal user, UpdateTicketDto updateTicketDto)
    {
        var ticketTemplate =
            await _unitOfWork.TicketTemplateRepository.GetAsync(t =>
                t.TicketTemplateId == updateTicketDto.TicketTemplateId);

        if (ticketTemplate == null)
            return new ResponseDto
            {
                Message = "TicketTemplate not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };

        var hasPurchasedTickets = await _unitOfWork.TicketRepository.GetAllAsync(t =>
            t.TicketTemplateId == ticketTemplate.TicketTemplateId);

        if (hasPurchasedTickets.Any())
            return new ResponseDto
            {
                Message = "Cannot update TicketTemplate. Tickets have already been purchased.",
                Result = null,
                IsSuccess = false,
                StatusCode = 400
            };


        // Update only the fields that are not null
        if (updateTicketDto.TicketName != null) ticketTemplate.TicketName = updateTicketDto.TicketName;
        if (updateTicketDto.EventId.HasValue) ticketTemplate.EventId = updateTicketDto.EventId.Value;
        if (updateTicketDto.TicketPrice.HasValue) ticketTemplate.TicketPrice = updateTicketDto.TicketPrice.Value;
        if (updateTicketDto.TicketImage != null) ticketTemplate.ImageTicket = updateTicketDto.TicketImage;
        if (updateTicketDto.Rank != null) ticketTemplate.Rank = updateTicketDto.Rank;
        if (updateTicketDto.IsVisible.HasValue) ticketTemplate.IsValid = updateTicketDto.IsVisible.Value;
        if (updateTicketDto.TotalQuantity.HasValue)
        {
            if (updateTicketDto.TotalQuantity.Value < ticketTemplate.AvailableQuantity)
                return new ResponseDto
                {
                    Message = "TotalQuantity cannot be less than AvailableQuantity",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 400
                };

            ticketTemplate.TotalQuantity = updateTicketDto.TotalQuantity.Value;
        }


        _unitOfWork.TicketTemplateRepository.Update(ticketTemplate);
        var saveResult = await _unitOfWork.SaveAsync();

        if (saveResult <= 0)
            return new ResponseDto
            {
                Message = "Failed to update TicketTemplate",
                Result = null,
                IsSuccess = false,
                StatusCode = 400
            };

        return new ResponseDto
        {
            Message = "TicketTemplate updated successfully",
            Result = ticketTemplate,
            IsSuccess = true,
            StatusCode = 200
        };
    }
/*public async Task<ResponseDto> CreateTicketByCustomer(ClaimsPrincipal user, CreateTicketDto createTicketDto)
{
    var userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
    if (userId == null)
        return new ResponseDto
        {
            Message = "User not found",
            Result = null,
            IsSuccess = false,
            StatusCode = 404
        };

    var serinumber =
        await _unitOfWork.TicketRepository.GetAsync(s => s.SerialNumber == createTicketDto.SerialNumber);
    if (serinumber != null)
        return new ResponseDto
        {
            Message = "Serial number already exists",
            Result = null,
            IsSuccess = false,
            StatusCode = 400
        };

    var ticket = new Ticket
    {
        TicketId = Guid.NewGuid(),
        TicketName = createTicketDto.TicketName,
        EventId = createTicketDto.EventId,
        UserId = userId,
        CategoryId = createTicketDto.CategoryId,
        TicketPrice = createTicketDto.TicketPrice,
        TicketImage = createTicketDto.TicketImage,
        TicketDescription = createTicketDto.TicketDescription,
        SerialNumber = createTicketDto.SerialNumber,
        NewPrice = 0,
        NegotiationStatus = false,
        Status = TicketStatus.Processing,
        IsVisible = true
    };

    await _unitOfWork.TicketRepository.AddAsync(ticket);
    await _unitOfWork.SaveAsync();

    return new ResponseDto
    {
        Message = "Ticket created successfully",
        IsSuccess = true,
        StatusCode = 201,
        Result = ticket
    };
}*/
/*

public async Task<ResponseDto> GetTicket(ClaimsPrincipal user, Guid ticketId)
{
var ticketById = await _unitOfWork.TicketRepository.GeTicketById(ticketId);
if (ticketById == null)
    return new ResponseDto
    {
        Message = "Ticket not found",
        Result = "",
        IsSuccess = false,
        StatusCode = 200
    };

GetTicketDto ticketDto;
ticketDto = _mapper.Map<GetTicketDto>(ticketById);

return new ResponseDto
{
    Message = "Get Ticket successfully",
    Result = ticketDto,
    IsSuccess = true,
    StatusCode = 201
};
}

public async Task<ResponseDto> GetTicketByUserId(ClaimsPrincipal user)
{
var userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
if (userId == null)
    return new ResponseDto
    {
        Message = "User not found",
        Result = null,
        IsSuccess = false,
        StatusCode = 404
    };

// Truy vấn tất cả vé của người dùng từ cơ sở dữ liệu
var tickets = await _unitOfWork.TicketRepository.GetAllAsync(x => x.UserId == userId && x.IsVisible);

if (tickets == null || !tickets.Any())
    return new ResponseDto
    {
        Message = "No tickets found for the user",
        Result = null,
        IsSuccess = false,
        StatusCode = 404
    };

// Ánh xạ vé lấy được từ cơ sở dữ liệu thành DTO
var ticketDtos = _mapper.Map<List<GetTicketDto>>(tickets);

return new ResponseDto
{
    Message = "Get Ticket successfully",
    Result = ticketDtos,
    IsSuccess = true,
    StatusCode = 200
};
}

public async Task<ResponseDto> CreateTicketByOrganiztion(ClaimsPrincipal user,
List<CreateTicketDto> createTicketDtos)
{
var userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
if (userId == null)
    return new ResponseDto
    {
        Message = "User not found",
        Result = null,
        IsSuccess = false,
        StatusCode = 404
    };

var tickets = new List<Ticket>();

foreach (var createTicketDto in createTicketDtos)
{
    var existingTicket = await _unitOfWork.TicketRepository
        .GetAsync(s => s.SerialNumber == createTicketDto.SerialNumber);
    if (existingTicket != null)
        return new ResponseDto
        {
            Message = $"Serial number {createTicketDto.SerialNumber} already exists",
            Result = null,
            IsSuccess = false,
            StatusCode = 400
        };

    var ticket = new Ticket
    {
        TicketId = Guid.NewGuid(),
        TicketName = createTicketDto.TicketName,
        EventId = createTicketDto.EventId,
        UserId = userId,
        CategoryId = createTicketDto.CategoryId,
        TicketPrice = createTicketDto.TicketPrice,
        TicketImage = createTicketDto.TicketImage,
        TicketDescription = createTicketDto.TicketDescription,
        SerialNumber = createTicketDto.SerialNumber,
        NewPrice = 0,
        NegotiationStatus = false,
        Status = TicketStatus.Success,
        IsVisible = true
    };

    tickets.Add(ticket);
}

await _unitOfWork.TicketRepository.AddRangeAsync(tickets);
await _unitOfWork.SaveAsync();

return new ResponseDto
{
    Message = "Tickets created successfully",
    IsSuccess = true,
    StatusCode = 201,
    Result = tickets
};
}



public async Task<ResponseDto> DeleteTicket(ClaimsPrincipal user, Guid ticketId)
{
var tId = await _unitOfWork.TicketRepository.GetAsync(c => c.TicketId == ticketId);

if (tId == null)
    return new ResponseDto
    {
        Message = "Delete level successfully",
        Result = null,
        IsSuccess = false,
        StatusCode = 400
    };

tId.IsVisible = false;

_unitOfWork.TicketRepository.Update(tId);
await _unitOfWork.SaveAsync();

return new ResponseDto
{
    Message = "Ticket deleted successfully",
    Result = tId,
    IsSuccess = true,
    StatusCode = 201
};
}

public async Task<ResponseDto> AcceptTicket(ClaimsPrincipal user, Guid ticketId)
{
var ticket = await _unitOfWork.TicketRepository.GetAsync(t => t.TicketId == ticketId);
if (ticket == null)
    return new ResponseDto
    {
        Message = "Ticket not found",
        Result = null,
        IsSuccess = false,
        StatusCode = 404
    };

ticket.Status = TicketStatus.Success;
_unitOfWork.TicketRepository.Update(ticket);
await _unitOfWork.SaveAsync();

return new ResponseDto
{
    Message = "Ticket Accepted successfully",
    Result = null,
    IsSuccess = true,
    StatusCode = 200
};
}

public async Task<ResponseDto> RejectTicket(ClaimsPrincipal user, Guid ticketId)
{
var ticket = await _unitOfWork.TicketRepository.GetAsync(t => t.TicketId == ticketId);
if (ticket == null)
    return new ResponseDto
    {
        Message = "Ticket not found",
        Result = null,
        IsSuccess = false,
        StatusCode = 404
    };

ticket.Status = TicketStatus.Rejected;
_unitOfWork.TicketRepository.Update(ticket);
await _unitOfWork.SaveAsync();

return new ResponseDto
{
    Message = "Ticket Rejected successfully",
    Result = null, IsSuccess = true,
    StatusCode = 200
};
}

public async Task<ResponseDto> GenerateQRCode(Guid ticketId, string serialNumber)
{
var qrData = new { ticketId, serialNumber };
var qrContent =
    $"tickethubapp.azurewebsites.net/api/Tickets/scan-qr-code?ticketId={Uri.EscapeDataString(ticketId.ToString())}&serialNumber={Uri.EscapeDataString(serialNumber)}";

using (var qrGenerator = new QRCodeGenerator())
{
    var qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
    using (var qrCode = new QRCode(qrCodeData))
    {
        using (var qrBitmap = qrCode.GetGraphic(20))
        {
            using (var ms = new MemoryStream())
            {
                qrBitmap.Save(ms, ImageFormat.Png);

                return new ResponseDto
                {
                    Message = "QR Code generated successfully",
                    Result = Convert.ToBase64String(ms.ToArray()),
                    IsSuccess = true,
                    StatusCode = 200
                };
            }
        }
    }
}
}

public async Task<ResponseDto> ValidateAndUpdateTicket(Guid ticketId, string serialNumber)
{
// Tìm vé trong database
var ticket = await _unitOfWork.TicketRepository.GeTicketById(ticketId);

if (ticket == null || ticket.SerialNumber != serialNumber)
    return new ResponseDto
    {
        Message = "Invalid QR Code or Ticket not found",
        Result = null,
        IsSuccess = false,
        StatusCode = 400
    }; // Vé không tồn tại hoặc SerialNumber không đúng

// Cập nhật trạng thái iVisible từ 1 thành 0
ticket.IsVisible = false;
_unitOfWork.TicketRepository.Update(ticket);

// Lưu thay đổi
await _unitOfWork.TicketRepository.SaveAsync();
return new ResponseDto
{
    Message = "Ticket validate QR successfully",
    Result = "https://tickethub-9f8e9.web.app/scan-qr-code",
    IsSuccess = true,
    StatusCode = 200
};
}
*/
}