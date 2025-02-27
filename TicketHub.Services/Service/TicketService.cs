using System.Drawing;
using System.Security.Claims;
using AutoMapper;
using Newtonsoft.Json;
using QRCoder.Core;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Ticket;
using TicketHub.Services.IService;
using TicketHub.Utility.Constants;

namespace TicketHub.Services.Service;

public class TicketService : ITicketService
{
    private readonly IUnitOfWork _unitOfWork;
    private IMapper _mapper;
    private readonly IFirebaseService _firebaseService;


    public TicketService(IUnitOfWork unitOfWork, IMapper mapper, IFirebaseService firebaseService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _firebaseService = firebaseService;
    }

    public async Task<ResponseDto> GetTickets
    (
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        // Lấy tất cả các vé có trong database
        var tickets = await _unitOfWork.TicketRepository.GetAllWithEventAndLocationAsync();

        // Kiểm tra nếu danh sách tickets là null hoặc rỗng
        if (!tickets.Any())
        {
            return new ResponseDto()
            {
                Message = "There are no tickets",
                IsSuccess = true,
                StatusCode = 404,
                Result = null
            };
        }

        bool isStaff = user.IsInRole("STAFF");
        string userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Lọc danh sách vé  
        List<Ticket> listTickets;

        if (isStaff)
        {
            // Nếu là staff, lấy tất cả vé  
            listTickets = tickets.ToList();
        }
        else
        {
            // Nếu là khách, chỉ lấy vé đã được duyệt và thấy được  
            listTickets = tickets.Where(ticket => ticket.Status == TicketStatus.Success && ticket.IsVisible).ToList();
        }


        // Filter Query
        if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
        {
            switch (filterOn.Trim().ToLower())
            {
                case "ticketname":
                    listTickets = listTickets.Where(x =>
                        x.TicketName.Contains(filterQuery, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    break;

                case "ticketprice":
                    if (double.TryParse(filterQuery, out double price))
                    {
                        listTickets = listTickets.Where(x => x.TicketPrice == price).ToList();
                    }

                    break;

                case "ticketdescription":
                    listTickets = listTickets.Where(x =>
                        x.TicketDescription.Contains(filterQuery, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    break;

                default:
                    break;
            }
        }

        // Sort Query
        if (!string.IsNullOrEmpty(sortBy))
        {
            switch (sortBy.Trim().ToLower())
            {
                case "ticketname":
                    listTickets = listTickets.OrderBy(x => x.TicketName).ToList();
                    break;

                case "ticketprice":
                    listTickets = listTickets.OrderBy(x => x.TicketPrice).ToList();
                    break;

                default:
                    // Nếu không có giá trị `sortBy` hợp lệ, sắp xếp theo `TicketPrice` giảm dần
                    listTickets = listTickets.OrderByDescending(x => x.TicketPrice).ToList();
                    break;
            }
        }
        else
        {
            // Trường hợp không có `sortBy` nào được chỉ định
            listTickets = listTickets.OrderByDescending(x => x.TicketPrice).ToList();
        }

        // Phân trang
        if (pageNumber > 0 && pageSize > 0)
        {
            var skipResult = (pageNumber - 1) * pageSize;
            listTickets = listTickets.Skip(skipResult).Take(pageSize).ToList();
        }

        // Chuyển đổi danh sách vé thành DTO
        var ticketsDto = listTickets.Select(ticket => new GetTicketDto()
        {
            TicketId = ticket.TicketId,
            TicketName = ticket.TicketName,
            TicketImage = ticket.TicketImage,
            EventId = ticket.EventId,
            EventName = ticket.Event?.EventName,
            UserId = ticket.UserId,
            CategoryId = ticket.CategoryId,
            CategoryName = ticket.Category?.CategoryName,
            TicketPrice = ticket.TicketPrice,
            NewPrice = ticket.NewPrice,
            TicketDescription = ticket.TicketDescription,
            SerialNumber = ticket.SerialNumber,
            City = ticket.Event.City,
            District = ticket.Event.District,
            Address = ticket.Event.Address,
            Status = ticket.Status,
            IsVisible = ticket.IsVisible,
            NegotiationStatus = ticket.NegotiationStatus,
            EventDate = ticket.Event?.EventDate ?? DateTime.MinValue
        }).ToList();

        return new ResponseDto()
        {
            Message = "Get Tickets successfully",
            IsSuccess = true,
            StatusCode = 200,
            Result = ticketsDto
        };
    }

    public async Task<ResponseDto> GetTicket(ClaimsPrincipal user, Guid ticketId)
    {
        var ticketById = await _unitOfWork.TicketRepository.GeTicketById(ticketId);
        if (ticketById == null)
        {
            return new ResponseDto
            {
                Message = "Ticket not found",
                Result = "",
                IsSuccess = false,
                StatusCode = 200
            };
        }

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
        {
            return new ResponseDto
            {
                Message = "User not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }

        // Truy vấn tất cả vé của người dùng từ cơ sở dữ liệu
        var tickets = await _unitOfWork.TicketRepository.GetAllAsync(x => x.UserId == userId && x.IsVisible);

        if (tickets == null || !tickets.Any())
        {
            return new ResponseDto
            {
                Message = "No tickets found for the user",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }

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

    public async Task<ResponseDto> CreateTicketByCustomer(ClaimsPrincipal user, CreateTicketDto createTicketDto)
    {
        var userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return new ResponseDto
            {
                Message = "User not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }

        var serinumber =
            await _unitOfWork.TicketRepository.GetAsync(s => s.SerialNumber == createTicketDto.SerialNumber);
        if (serinumber != null)
        {
            return new ResponseDto
            {
                Message = "Serial number already exists",
                Result = null,
                IsSuccess = false,
                StatusCode = 400
            };
        }

        Ticket ticket = new Ticket()
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

        return new ResponseDto()
        {
            Message = "Ticket created successfully",
            IsSuccess = true,
            StatusCode = 201,
            Result = ticket
        };
    }

    public async Task<ResponseDto> CreateTicketByOrganiztion(ClaimsPrincipal user,
        List<CreateTicketDto> createTicketDtos)
    {
        var userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return new ResponseDto
            {
                Message = "User not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }

        List<Ticket> tickets = new List<Ticket>();

        foreach (var createTicketDto in createTicketDtos)
        {
            var existingTicket = await _unitOfWork.TicketRepository
                .GetAsync(s => s.SerialNumber == createTicketDto.SerialNumber);
            if (existingTicket != null)
            {
                return new ResponseDto
                {
                    Message = $"Serial number {createTicketDto.SerialNumber} already exists",
                    Result = null,
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            Ticket ticket = new Ticket()
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

        return new ResponseDto()
        {
            Message = "Tickets created successfully",
            IsSuccess = true,
            StatusCode = 201,
            Result = tickets
        };
    }

    public async Task<ResponseDto> UpdateTicket(ClaimsPrincipal user, UpdateTicketDto updateTicketDto)
    {
        var ticketId = await _unitOfWork.TicketRepository.GetAsync(c => c.TicketId == updateTicketDto.TicketId);

        // kiểm tra xem có null không
        if (ticketId == null)
        {
            return new ResponseDto
            {
                Message = "Ticket not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }

        // cập nhật thông tin danh mục
        ticketId.TicketName = updateTicketDto.TicketName;
        ticketId.TicketDescription = updateTicketDto.TicketDescription;
        ticketId.TicketPrice = updateTicketDto.TicketPrice;
        ticketId.TicketImage = updateTicketDto.TicketImage;
        ticketId.SerialNumber = updateTicketDto.SerialNumber;
        ticketId.EventId = updateTicketDto.EventId;
        ticketId.CategoryId = updateTicketDto.CategoryId;
        ticketId.Status = TicketStatus.Processing;
        ticketId.IsVisible = true;


        // thay đổi dữ liệu
        _unitOfWork.TicketRepository.Update(ticketId);

        // lưu thay đổi vào cơ sở dữ liệu
        var save = await _unitOfWork.SaveAsync();
        if (save <= 0)
        {
            return new ResponseDto
            {
                Message = "Failed to update ticket",
                Result = null,
                IsSuccess = false,
                StatusCode = 400
            };
        }

        return new ResponseDto
        {
            Message = "Ticket updated successfully",
            Result = ticketId,
            IsSuccess = true,
            StatusCode = 201
        };
    }

    public async Task<ResponseDto> DeleteTicket(ClaimsPrincipal user, Guid ticketId)
    {
        var tId = await _unitOfWork.TicketRepository.GetAsync(c => c.TicketId == ticketId);

        if (tId == null)
        {
            return new ResponseDto
            {
                Message = "Delete level successfully",
                Result = null,
                IsSuccess = false,
                StatusCode = 400
            };
        }

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

    public async Task<ResponseDto> UploadTicketImage(
        ClaimsPrincipal user,
        UploadTicketImgDto uploadTicketImgDto
    )
    {
        if (uploadTicketImgDto.File == null)
        {
            return new ResponseDto()
            {
                IsSuccess = false,
                StatusCode = 400,
                Message = "No file uploaded."
            };
        }

        // Upload image lên Firebase và nhận URL công khai
        var responseDto =
            await _firebaseService.UploadImageTicket(uploadTicketImgDto.File, StaticFirebaseFolders.TicketImages);

        if (!responseDto.IsSuccess)
        {
            return new ResponseDto()
            {
                Message = "Image upload failed!",
                Result = null,
                IsSuccess = false,
                StatusCode = 400 // Bad Request
            };
        }

        // Trả về link công khai của hình ảnh
        return new ResponseDto()
        {
            Message = "Upload ticket image successfully!",
            Result = responseDto.Result, // Đảm bảo đây là URL công khai của ảnh đã upload
            IsSuccess = true,
            StatusCode = 200 // OK
        };
    }

    public async Task<ResponseDto> AcceptTicket(ClaimsPrincipal user, Guid ticketId)
    {
        var ticket = await _unitOfWork.TicketRepository.GetAsync(t => t.TicketId == ticketId);
        if (ticket == null)
        {
            return new ResponseDto()
            {
                Message = "Ticket not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }

        ticket.Status = TicketStatus.Success;
        _unitOfWork.TicketRepository.Update(ticket);
        await _unitOfWork.SaveAsync();

        return new ResponseDto()
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
        {
            return new ResponseDto()
            {
                Message = "Ticket not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }

        ticket.Status = TicketStatus.Rejected;
        _unitOfWork.TicketRepository.Update(ticket);
        await _unitOfWork.SaveAsync();

        return new ResponseDto()
        {
            Message = "Ticket Rejected successfully",
            Result = null, IsSuccess = true,
            StatusCode = 200
        };
    }
    
    public async Task<ResponseDto> GenerateQRCode(Guid ticketId, string serialNumber)
    {
        var qrData = new { ticketId, serialNumber };
        string qrContent = $"tickethubapp.azurewebsites.net/api/Tickets/scan-qr-code?ticketId={Uri.EscapeDataString(ticketId.ToString())}&serialNumber={Uri.EscapeDataString(serialNumber)}";

        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        {
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
            using (QRCode qrCode = new QRCode(qrCodeData))
            {
                using (Bitmap qrBitmap = qrCode.GetGraphic(20))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        qrBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                        return new ResponseDto()
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
        {
            return new ResponseDto()
            {
                Message = "Invalid QR Code or Ticket not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 400
            }; // Vé không tồn tại hoặc SerialNumber không đúng
        }

        // Cập nhật trạng thái iVisible từ 1 thành 0
        ticket.IsVisible = false;
        _unitOfWork.TicketRepository.Update(ticket);

        // Lưu thay đổi
        await _unitOfWork.TicketRepository.SaveAsync();
        return new ResponseDto()
        {
            Message = "Ticket validate QR successfully",
            Result = "https://tickethub-9f8e9.web.app/scan-qr-code",
            IsSuccess = true,
            StatusCode = 200
        };
    }
}