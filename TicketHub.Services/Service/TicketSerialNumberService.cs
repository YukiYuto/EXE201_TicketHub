using System.Security.Claims;
using AutoMapper;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.TicketSerialNumber;
using TicketHub.Services.IService;

namespace TicketHub.Services.Service;

public class TicketSerialNumberService : ITicketSerialNumberService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public TicketSerialNumberService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseDto> CreateTicketSerialNumber(ClaimsPrincipal user,
        List<CreateTicketSerialNumberDto> createTicketSerialNumberDto)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new ResponseDto
            {
                Message = "User not found",
                IsSuccess = false,
                StatusCode = 404
            };

        var ticketTemplateIds = createTicketSerialNumberDto.Select(dto => dto.TicketTemplateId).Distinct().ToList();

        var ticketTemplates =
            await _unitOfWork.TicketTemplateRepository.GetListAsync(t =>
                ticketTemplateIds.Contains(t.TicketTemplateId));

        var ticketSerialNumbers = new List<TicketSerialNumber>();

        foreach (var ticketTemplate in ticketTemplates)
        {
            var serialNumbersSet = new HashSet<string>();

            while (serialNumbersSet.Count < ticketTemplate.TotalQuantity)
            {
                var serialNumber = GenerateRandomSerial();
                if (!serialNumbersSet.Contains(serialNumber)) serialNumbersSet.Add(serialNumber);
            }

            ticketSerialNumbers.AddRange(serialNumbersSet.Select(serial => new TicketSerialNumber
            {
                SerialNumberId = Guid.NewGuid(),
                TicketTemplateId = ticketTemplate.TicketTemplateId,
                SerialNumber = serial,
                CreatedBy = user.FindFirstValue(ClaimTypes.Name),
                CreatedTime = DateTime.UtcNow.AddHours(7),
                Status = "ACTIVE"
            }));
        }

        await _unitOfWork.TicketSerialNumberRepository.AddRangeAsync(ticketSerialNumbers);
        await _unitOfWork.SaveAsync();

        var resultDto = ticketSerialNumbers.Select(tsn => new GetTicketSerialNumberDto
        {
            SerialNumberId = tsn.SerialNumberId,
            TicketTemplateId = tsn.TicketTemplateId,
            SerialNumber = tsn.SerialNumber
        }).ToList();

        return new ResponseDto
        {
            Message = "Ticket Serial Numbers created successfully",
            IsSuccess = true,
            StatusCode = 201,
            Result = resultDto
        };
    }

    public async Task<ResponseDto> GetTicketSerialNumberById(ClaimsPrincipal user, Guid serialNumberId)
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
                Message = "You are not an Organizer",
                IsSuccess = false,
                StatusCode = 404
            };

        var ticketSerialNumber =
            await _unitOfWork.TicketSerialNumberRepository.GetAsync(tsn => tsn.SerialNumberId == serialNumberId);

        if (ticketSerialNumber == null)
            return new ResponseDto
            {
                Message = "Ticket Serial Number not found",
                IsSuccess = false,
                StatusCode = 404
            };

        var resultDto = _mapper.Map<GetTicketSerialNumberDto>(ticketSerialNumber);

        return new ResponseDto
        {
            Message = "Ticket Serial Number found successfully",
            IsSuccess = true,
            StatusCode = 200,
            Result = resultDto
        };
    }

    public async Task<ResponseDto> UpdateTicketSerialNumber(ClaimsPrincipal user,
        UpdateTicketSerialNumberDto updateTicketSerialNumberDto)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new ResponseDto
            {
                Message = "User not found",
                IsSuccess = false,
                StatusCode = 404
            };

        var ticketSerialNumber = await
            _unitOfWork.TicketSerialNumberRepository.GetAsync(tsn =>
                tsn.SerialNumberId == updateTicketSerialNumberDto.SerialNumberId);

        if (ticketSerialNumber == null)
            return new ResponseDto
            {
                Message = "Ticket Serial Number not found",
                IsSuccess = false,
                StatusCode = 404
            };

        _mapper.Map(updateTicketSerialNumberDto, ticketSerialNumber);
        ticketSerialNumber.UpdatedBy = user.FindFirstValue(ClaimTypes.Name);
        ticketSerialNumber.UpdatedTime = DateTime.UtcNow.AddHours(7);

        _unitOfWork.TicketSerialNumberRepository.Update(ticketSerialNumber);
        await _unitOfWork.SaveAsync();

        return new ResponseDto
        {
            Message = "Ticket Serial Number updated successfully",
            IsSuccess = true,
            StatusCode = 200,
            Result = ticketSerialNumber
        };
    }

    public Task<ResponseDto> DeleteTicketSerialNumber(ClaimsPrincipal user, Guid serialNumberId)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseDto> GetTicketSerialNumbers
    (
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 1,
        int pageSize = 10,
        Guid? ticketTemplateId = null
    )
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new ResponseDto
            {
                Message = "User not found",
                IsSuccess = false,
                StatusCode = 404
            };

        var orgainer = await _unitOfWork.OrganizationRepository.GetAsync(o => o.UserId == userId);
        if (orgainer == null)
            return new ResponseDto
            {
                Message = "You are not an Organizer",
                IsSuccess = false,
                StatusCode = 404
            };

        // Kiểm tra TicketTemplateId có tồn tại không
        var ticketTemplate =
            await _unitOfWork.TicketTemplateRepository.GetAsync(tt => tt.TicketTemplateId == ticketTemplateId);
        if (ticketTemplate == null)
            return new ResponseDto
            {
                Message = "Ticket Template not found",
                IsSuccess = false,
                StatusCode = 404
            };

        var (ticketSerialNumbers, totalItems) =
            await _unitOfWork.TicketSerialNumberRepository.GetTicketSerialNumberAsync(
                filterOn,
                filterQuery,
                sortBy,
                pageNumber,
                pageSize,
                ticketTemplateId
            );

        var resultDto = ticketSerialNumbers.Select(tsn => _mapper.Map<GetTicketSerialNumberDto>(tsn)).ToList();

        return new ResponseDto
        {
            Message = "Ticket Serial Numbers found successfully",
            IsSuccess = true,
            StatusCode = 200,
            Result = new
            {
                totalItems,
                resultDto
            }
        };
    }

// Hàm tạo số serial ngẫu nhiên 6 chữ số
    private string GenerateRandomSerial()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}