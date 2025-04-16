using System.Drawing.Imaging;
using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QRCoder.Core;
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

    public async Task<ResponseDto> GetTicketTemplates(
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

    public async Task<ResponseDto> GetTicketTemplateByUserId(ClaimsPrincipal user)
    {
        /*// Lấy userId từ ClaimsPrincipal
        var userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return new ResponseDto
            {
                Message = "Bạn chưa đăng nhập",
                Result = null,
                IsSuccess = false,
                StatusCode = 401
            };

        // Lấy Organizer và ApplicationUser cùng lúc bằng Include
        var organizer = await _unitOfWork.OrganizationRepository.GetAsync(
            o => o.UserId == userId,
            include: query => query.Include(o => o.User)
        );
        if (organizer == null)
            return new ResponseDto
            {
                Message = "Bạn không phải là người tổ chức",
                Result = null,
                IsSuccess = false,
                StatusCode = 403
            };

        // Kiểm tra email của Organizer
        if (organizer.User == null || string.IsNullOrEmpty(organizer.User.Email))
            return new ResponseDto
            {
                Message = "Không tìm thấy thông tin email của người tổ chức",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };

        // Lấy TicketTemplate mà Event.CreatedBy khớp với email của Organizer
        var ticketTemplates = await _unitOfWork.TicketTemplateRepository.GetAllAsync(
            x => x.Event.CreatedBy == organizer.User.Email,
            ticket => ticket.Include(t => t.Event)
                .ThenInclude(e => e.Category)
        );

        return new ResponseDto
        {
            Message = "Lấy danh sách mẫu vé thành công",
            Result = ticketTemplates,
            IsSuccess = true,
            StatusCode = 200
        };*/
        return null;
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

    public async Task<ResponseDto> GenerateQrCode(Guid ticketId, Guid serialNumberId)
    {
        var qrData = new { ticketId, serialNumberId };
        var qrContent =
            $"tickethub-fpgfa9ara4b6czbe.southeastasia-01.azurewebsites.net/api/Tickets/scan-qr-code?ticketId={Uri.EscapeDataString(ticketId.ToString())}&serialNumber={Uri.EscapeDataString(serialNumberId.ToString())}";

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

    public async Task<ResponseDto> ValidateAndUpdateTicket(Guid ticketId, Guid serialNumberId)
    {
        // Tìm vé trong database
        var ticket = await _unitOfWork.TicketRepository.GeTicketById(ticketId);

        if (ticket == null || ticket.SerialNumberId != serialNumberId)
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
        await _unitOfWork.SaveAsync();

        var result = new Ticket
        {
            TicketId = ticket.TicketId,
            TicketTemplate = new TicketTemplate
            {
                TicketName = ticket.TicketTemplate.TicketName,
                Event = new Event
                {
                    EventName = ticket.TicketTemplate.Event.EventName
                },
                TicketPrice = ticket.TicketTemplate.TicketPrice,
                ImageTicket = ticket.TicketTemplate.ImageTicket,
                Rank = ticket.TicketTemplate.Rank
            }
        };

        return new ResponseDto
        {
            Message = "Ticket validate QR successfully",
            Result = result,
            /*Result = "https://tickethub-9f8e9.web.app/scan-qr-code",*/
            IsSuccess = true,
            StatusCode = 200
        };
    }

    public async Task<ResponseDto> GetTicketTemplateByEventId(Guid eventId)
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
}