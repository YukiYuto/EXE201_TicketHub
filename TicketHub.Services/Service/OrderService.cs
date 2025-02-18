using System.Security.Claims;
using AutoMapper;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Order;
using TicketHub.Services.IService;

namespace TicketHub.Services.Service;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    public async Task<ResponseDto> GetOrders(ClaimsPrincipal user, string? filterOn, string? filterQuery, string? sortBy, int pageNumber = 1, int pageSize = 10)
    {
        IEnumerable<Orders> allOrders = null!;
        allOrders = await _unitOfWork.OrderRepository.GetAllAsync();
        
        if (!allOrders.Any())
        {
            return new ResponseDto()
            {
                Message = "There are no orders",
                IsSuccess = true,
                StatusCode = 404,
                Result = null
            };
        }
        
        var listOrders = allOrders.ToList();

        if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
        {
            switch (filterOn.Trim().ToLower())
            {   
                case "TotalPrice":
                    listOrders = listOrders.Where(x => x.TotalPrice.ToString().Contains(filterQuery, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    break;
                default:
                    break;
            }
        }
        
        if (!string.IsNullOrEmpty(sortBy))
        {
            var sortParams = sortBy.Trim().ToLower().Split('_'); // Chia chuỗi sortBy theo ký tự '_'
            var sortField = sortParams[0]; // Tên cột cần sắp xếp
            var sortDirection = sortParams.Length > 1 ? sortParams[1] : "asc"; // Lấy hướng sắp xếp

            switch (sortField)
            {
                case "TotalPrice":
                    listOrders = sortDirection.Equals("desc") 
                        ? listOrders.OrderByDescending(x => x.TotalPrice).ToList() 
                        : listOrders.OrderBy(x => x.TotalPrice).ToList();
                    break;
                
                default:
                    listOrders = listOrders.OrderBy(x => x.TotalPrice).ToList();
                    break;
            }
        }
        else
        {
            listOrders = listOrders.OrderBy(x => x.TotalPrice).ToList();
        }
        
        // Phân trang
        if (pageNumber > 0 && pageSize > 0)
        {
            var skipResult = (pageNumber - 1) * pageSize;
            listOrders = listOrders.Skip(skipResult).Take(pageSize).ToList();
        }
        
        // Chuyển đổi danh sách sự kiện thành DTO
        var orderDto = listOrders.Select(eventItem => new GetOrderDto()
        {
            OrderId = eventItem.OrderId,
            UserId = eventItem.UserId,
            TotalPrice = eventItem.TotalPrice
        }).ToList();

        return new ResponseDto()
        {
            Message = "Get orders successfully",
            IsSuccess = true,
            StatusCode = 200,
            Result = orderDto
        };
    }

    public async Task<ResponseDto> GetOrder(ClaimsPrincipal user, Guid orderId)
    {
        var orderID = await _unitOfWork.OrderRepository.GetById(orderId);
        if (orderID == null)
        {
            return new ResponseDto
            {
                Message = "Order not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }

        var orderDto = _mapper.Map<GetOrderDto>(orderID);

        return new ResponseDto
        {
            Message = "Order found successfully",
            Result = orderDto,
            IsSuccess = true,
            StatusCode = 201
        };
    }

    public async Task<ResponseDto> CreateOrder(ClaimsPrincipal user, CreateOrderDto createOrderDto)
    {
        Orders newOrder = new Orders()
        {
            OrderId = createOrderDto.OrderId,
            UserId = createOrderDto.UserId,
            TotalPrice = createOrderDto.TotalPrice
        };
        
        await _unitOfWork.OrderRepository.AddAsync(newOrder);
        await _unitOfWork.SaveAsync();

        return new ResponseDto
        {
            Message = "Order created successfully",
            Result = _mapper.Map<GetOrderDto>(newOrder),
            IsSuccess = true,
            StatusCode = 201
        };
    }

    public async Task<ResponseDto> UpdateOrder(ClaimsPrincipal user, UpdateOrderDto updateOrderDto)
    {
        var orderId = await _unitOfWork.OrderRepository.GetAsync(x => x.OrderId == updateOrderDto.OrderId);
        if (orderId == null)
        {
            return new ResponseDto
            {
                Message = "Order not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }
        
        orderId.UserId = updateOrderDto.UserId;
        orderId.TotalPrice = updateOrderDto.TotalPrice;
        
        _unitOfWork.OrderRepository.Update(orderId);
        var order = await _unitOfWork.SaveAsync();

        return new ResponseDto
        {
            Message = "Order updated successfully",
            Result = _mapper.Map<GetOrderDto>(orderId),
            IsSuccess = true,
            StatusCode = 200
        };
    }

    public async Task<ResponseDto> DeleteOrder(ClaimsPrincipal user, Guid orderId)
    {
        var order = await _unitOfWork.OrderRepository.GetAsync(x => x.OrderId == orderId);
        if (order == null)
        {
            return new ResponseDto
            {
                Message = "Order not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }
        
        _unitOfWork.OrderRepository.Remove(order);
        var delete = await _unitOfWork.SaveAsync();
        
        return new ResponseDto
        {
            Message = "Order deleted successfully",
            Result = null,
            IsSuccess = true,
            StatusCode = 200
        };
    }
}