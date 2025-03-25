using System.Security.Claims;
using AutoMapper;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Event;
using TicketHub.Services.IService;

namespace TicketHub.Services.Service;

public class EventService : IEventService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public EventService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseDto> GetEvents
    (
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        IEnumerable<Event> allEvents = null!;

        // Lấy tất cả các sự kiện có trong database
        allEvents = await _unitOfWork.EventRepository.GetAllAsync();

        // Kiểm tra nếu danh sách events là null hoặc rỗng
        if (!allEvents.Any())
            return new ResponseDto
            {
                Message = "There are no events",
                IsSuccess = true,
                StatusCode = 404,
                Result = null
            };

        // Kiểm tra quyền của người dùng  
        var isStaff = user.IsInRole("STAFF");

        // Lọc danh sách sự kiện dựa vào quyền  
        if (!isStaff) allEvents = allEvents.Where(e => e.Status == 1);

        var listEvents = allEvents.ToList();

        // Filter Query
        if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
            switch (filterOn.Trim().ToLower())
            {
                case "eventname":
                    listEvents = listEvents.Where(x =>
                        x.EventName.Contains(filterQuery, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    break;
                case "eventdate":
                    if (DateTime.TryParse(filterQuery, out var filterDate))
                        listEvents = listEvents.Where(x =>
                                x.EventDate.Date >= filterDate.Date && x.EventDate.Date < filterDate.Date.AddDays(1))
                            .ToList();

                    break;
            }

        if (!string.IsNullOrEmpty(sortBy))
        {
            var sortParams = sortBy.Trim().ToLower().Split('_'); // Chia chuỗi sortBy theo ký tự '_'
            var sortField = sortParams[0]; // Tên cột cần sắp xếp
            var sortDirection = sortParams.Length > 1 ? sortParams[1] : "asc"; // Lấy hướng sắp xếp

            switch (sortField)
            {
                case "eventname":
                    listEvents = sortDirection == "desc"
                        ? listEvents.OrderByDescending(x => x.EventName).ToList()
                        : listEvents.OrderBy(x => x.EventName).ToList();
                    break;

                case "eventdate":
                    listEvents = sortDirection == "desc"
                        ? listEvents.OrderByDescending(x => x.EventDate).ToList()
                        : listEvents.OrderBy(x => x.EventDate).ToList();
                    break;

                default:
                    // Sắp xếp mặc định theo ngày gần nhất nếu không có cột phù hợp
                    listEvents = listEvents.OrderBy(x => x.EventDate).ToList();
                    break;
            }
        }
        else
        {
            // Sắp xếp mặc định theo ngày gần nhất nếu không có `sortBy`
            listEvents = listEvents.OrderBy(x => x.EventDate).ToList();
        }

        // Phân trang
        if (pageNumber > 0 && pageSize > 0)
        {
            var skipResult = (pageNumber - 1) * pageSize;
            listEvents = listEvents.Skip(skipResult).Take(pageSize).ToList();
        }

        var ticketTemplates = await _unitOfWork.TicketTemplateRepository.GetAllAsync();

        // Chuyển đổi danh sách sự kiện thành DTO
        var eventDto = listEvents.Select(eventItem => new GetEventDto
        {
            EventId = eventItem.EventId,
            EventName = eventItem.EventName,
            EventDate = eventItem.EventDate,
            Location = eventItem.Location,
            EventDescription = eventItem.EventDescription,
            Status = eventItem.Status,
            EventImage = eventItem.EventImage
        }).ToList();

        return new ResponseDto
        {
            Message = "Get Events successfully",
            IsSuccess = true,
            StatusCode = 200,
            Result = eventDto
        };
    }

    public async Task<ResponseDto> GetEvent(ClaimsPrincipal user, Guid eventId)
    {
        var eventEntity = await _unitOfWork.EventRepository.GetById(eventId);
        if (eventEntity == null)
            return new ResponseDto
            {
                Message = "Event not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };

        // Lấy danh sách ticket templates liên quan đến event
        var ticketTemplates = await _unitOfWork.TicketTemplateRepository.GetAllAsync(x => x.EventId == eventId);

        var eventDto = new GetEventDto
        {
            EventId = eventEntity.EventId,
            EventName = eventEntity.EventName,
            EventDate = eventEntity.EventDate,
            Location = eventEntity.Location,
            EventDescription = eventEntity.EventDescription,
            Status = eventEntity.Status,
            EventImage = eventEntity.EventImage,
            CategoryId = eventEntity.CategoryId
        };

        return new ResponseDto
        {
            Message = "Event found successfully",
            Result = eventDto,
            IsSuccess = true,
            StatusCode = 200
        };
    }

    public async Task<ResponseDto> CreateEvent(ClaimsPrincipal user, CreateEventDto createEventDto)
    {
        var eventId = Guid.NewGuid();

        var newEvent = new Event
        {
            EventId = eventId,
            EventName = createEventDto.EventName,
            EventDescription = createEventDto.EventDescription,
            EventDate = DateTime.SpecifyKind(createEventDto.EventDate, DateTimeKind.Utc).ToUniversalTime(),
            CreatedBy = user.Identity!.Name,
            Location = createEventDto.Location,
            CategoryId = createEventDto.CategoryId,
            UpdatedBy = "",
            CreatedTime = DateTime.UtcNow.AddHours(7),
            UpdatedTime = null,
            Status = 1,
            EventImage = createEventDto.EventImage
        };

        await _unitOfWork.EventRepository.AddAsync(newEvent);
        await _unitOfWork.SaveAsync();

        // Tạo đối tượng trả về
        var eventResponse = new
        {
            newEvent.EventId,
            newEvent.EventName,
            newEvent.EventDescription,
            newEvent.EventDate,
            newEvent.CreatedBy,
            newEvent.Location,
            newEvent.CategoryId,
            newEvent.Status,
            newEvent.EventImage
        };

        return new ResponseDto
        {
            Message = "Event created successfully",
            Result = eventResponse,
            IsSuccess = true,
            StatusCode = 201
        };
    }


    public async Task<ResponseDto> UpdateEvent(ClaimsPrincipal user, UpdateEventDto updateEventDto)
    {
        var eventId = await _unitOfWork.EventRepository.GetAsync(x => x.EventId == updateEventDto.EventId);
        if (eventId == null)
            return new ResponseDto
            {
                Message = "Event not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };

        //update location
        eventId.EventName = updateEventDto.EventName;
        eventId.EventDescription = updateEventDto.EventDescription;
        eventId.EventDate = updateEventDto.EventDate;
        eventId.Location = updateEventDto.Location;
        eventId.UpdatedBy = user.Identity!.Name;
        eventId.UpdatedTime = DateTime.UtcNow;
        eventId.EventImage = updateEventDto.EventImage;


        //save changes
        _unitOfWork.EventRepository.Update(eventId);
        var save = await _unitOfWork.SaveAsync();

        // Tạo đối tượng trả về
        var eventResponse = new
        {
            eventId.EventId,
            eventId.EventName,
            eventId.EventDescription,
            eventId.EventDate,
            eventId.CreatedBy,
            eventId.Location,
            eventId.CategoryId,
            eventId.Status,
            eventId.EventImage
        };

        return new ResponseDto
        {
            Message = "Event updated successfully",
            Result = eventResponse,
            IsSuccess = true,
            StatusCode = 201
        };
    }

    public async Task<ResponseDto> DeleteEvent(ClaimsPrincipal user, Guid eventId)
    {
        var events = await _unitOfWork.EventRepository.GetAsync(x => x.EventId == eventId);
        if (events == null)
            return new ResponseDto
            {
                Message = "Location not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };

        events.Status = 0;
        events.UpdatedBy = user.Identity.Name;
        events.UpdatedTime = DateTime.UtcNow;

        //save changes
        _unitOfWork.EventRepository.Update(events);
        var save = await _unitOfWork.SaveAsync();

        return new ResponseDto
        {
            Message = "Events delete successfully",
            Result = events,
            IsSuccess = true,
            StatusCode = 201
        };
    }

    public async Task<ResponseDto> SearchEvent(ClaimsPrincipal user, string eventName)
    {
        var events = await _unitOfWork.EventRepository.GetAsync(x => x.EventName.Contains(eventName));
        if (events == null)
            return new ResponseDto
            {
                Message = "Event not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };

        var listTicketTemplates =
            await _unitOfWork.TicketTemplateRepository.GetAllAsync(x => x.EventId == events.EventId);

        var eventDto = new GetEventDto
        {
            EventId = events.EventId,
            EventName = events.EventName,
            EventDate = events.EventDate,
            Location = events.Location,
            EventDescription = events.EventDescription,
            Status = events.Status,
            EventImage = events.EventImage,
            CategoryId = events.CategoryId
        };

        return new ResponseDto
        {
            Message = "Event found successfully",
            Result = eventDto,
            IsSuccess = true,
            StatusCode = 201
        };
    }

    public async Task<ResponseDto> GetEventByUserId(ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrEmpty(userId))
            return new ResponseDto
            {
                Message = "User ID not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 400
            };

        var events = await _unitOfWork.EventRepository.GetAsync(x => x.CreatedBy == userId);

        if (events == null)
            return new ResponseDto
            {
                Message = "Event not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };

        var eventDto = new GetEventDto
        {
            EventId = events.EventId,
            EventName = events.EventName,
            EventDate = events.EventDate,
            Location = events.Location,
            EventDescription = events.EventDescription,
            Status = events.Status,
            EventImage = events.EventImage,
            CategoryId = events.CategoryId
        };

        return new ResponseDto
        {
            Message = "Event found successfully",
            Result = eventDto,
            IsSuccess = true,
            StatusCode = 200
        };
    }
}