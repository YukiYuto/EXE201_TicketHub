using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Order;
using TicketHub.Services.IService;
using TicketHub.Utility.Constants;

namespace TicketHub.Services.Service;

public class OrderService : IOrderService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderService
    (
        IUnitOfWork unitOfWork,
        IMapper mapper,
        UserManager<ApplicationUser> userManager
    )
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
    }

    /*public async Task<ResponseDto> CreateOrder(ClaimsPrincipal user, CreateOrderDto createOrderDto)
    {
        try
        {
            var userId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new ResponseDto
                {
                    Message = "User not found",
                    IsSuccess = false,
                    StatusCode = 404
                };

            // Lấy danh sách CartItem đã checkout từ database
            var checkedOutCartItems = await _unitOfWork.CartItemRepository
                .GetAllAsync(x => createOrderDto.CheckedOutCartItemIds.Contains(x.CartItemId),
                    "Ticket");

            if (checkedOutCartItems == null || !checkedOutCartItems.Any())
                return new ResponseDto
                {
                    Message = "No cart items found for the provided IDs",
                    IsSuccess = false,
                    StatusCode = 400
                };

            // Tạo đơn hàng từ danh sách CartItem đã checkout
            var newOrder = new Orders
            {
                OrderId = Guid.NewGuid(),
                UserId = userId,
                TotalPrice = createOrderDto.CheckoutTotalPrice,
                OrderNumber = await _unitOfWork.OrderRepository.GenerateUniqueNumberAsync(),
                Status = "1"
            };

            // Tạo danh sách OrderDetail từ các vé đã checkout
            newOrder.OrderTickets = checkedOutCartItems.Select(cartItem => new OrderTicket
            {
                OrderId = newOrder.OrderId,
                TicketId = cartItem.TicketId
            }).ToList();

            // Lưu order vào database
            await _unitOfWork.OrderRepository.AddAsync(newOrder);
            await _unitOfWork.SaveAsync();

            // Chuyển đổi order sang DTO để trả về response
            var orderDto = _mapper.Map<GetOrderDto>(newOrder);

            return new ResponseDto
            {
                Message = "Order created successfully",
                Result = orderDto,
                IsSuccess = true,
                StatusCode = 201
            };
        }
        catch (Exception e)
        {
            return new ResponseDto
            {
                Message = e.Message,
                IsSuccess = false,
                StatusCode = 500
            };
        }
    }
    */


    public async Task<ResponseDto> GetOrders
    (
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        // Lấy UserId từ Claims
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return new ResponseDto
            {
                Message = "User is not authenticated",
                IsSuccess = false,
                StatusCode = 401
            };

        // Kiểm tra xem user có phải là Manager hay không
        var isManager = user.IsInRole(StaticUserRoles.Manager);

        // Lấy danh sách đơn hàng theo vai trò
        IEnumerable<Orders> allOrders;
        if (isManager)
        {
            // Nếu là Manager, lấy tất cả đơn hàng
            allOrders = await _unitOfWork.OrderRepository.GetAllAsync();
        }
        else
        {
            // Nếu là Customer, chỉ lấy đơn hàng của họ
            var customer = await _unitOfWork.CustomerRepository.GetAsync(c => c.UserId == userId);
            if (customer == null)
                return new ResponseDto
                {
                    Message = "Customer not found",
                    IsSuccess = false,
                    StatusCode = 404
                };

            allOrders = await _unitOfWork.OrderRepository.GetAllAsync(o =>
                o.CustomerId == customer.CustomerId && o.Status == "Pending");
        }

        if (!allOrders.Any())
            return new ResponseDto
            {
                Message = "There are no orders",
                IsSuccess = true,
                StatusCode = 200,
                Result = allOrders
            };

        var listOrders = allOrders.ToList();

        // Lọc theo query (nếu có)
        if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
            switch (filterOn.Trim().ToLower())
            {
                case "totalprice":
                    listOrders = listOrders
                        .Where(x => x.TotalPrice.ToString()
                            .Contains(filterQuery, StringComparison.CurrentCultureIgnoreCase))
                        .ToList();
                    break;
            }

        // Sắp xếp theo yêu cầu (nếu có)
        if (!string.IsNullOrEmpty(sortBy))
        {
            var sortParams = sortBy.Trim().ToLower().Split('_');
            var sortField = sortParams[0];
            var sortDirection = sortParams.Length > 1 ? sortParams[1] : "asc";

            switch (sortField)
            {
                case "totalprice":
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

        // Chuyển đổi danh sách đơn hàng thành DTO
        var orderDto = listOrders.Select(orderItem => new GetOrderDto
        {
            OrderId = orderItem.OrderId,
            CustomerId = orderItem.CustomerId,
            TotalPrice = orderItem.TotalPrice,
            OrderNumber = orderItem.OrderNumber,
            Status = orderItem.Status
        }).ToList();

        return new ResponseDto
        {
            Message = "Get orders successfully",
            IsSuccess = true,
            StatusCode = 200,
            Result = orderDto
        };
    }
/*
    public async Task<ResponseDto> GetOrder(ClaimsPrincipal user, Guid orderId)
    {
        var order = await _unitOfWork.OrderRepository.GetById(orderId);
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

        var orderDto = _mapper.Map<GetOrderDto>(order);

        return new ResponseDto
        {
            Message = "Order found successfully",
            Result = orderDto,
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



        await _unitOfWork.OrderRepository.AddAsync(order);
        var delete = await _unitOfWork.SaveAsync();

        return new ResponseDto
        {
            Message = "Order deleted successfully",
            Result = null,
            IsSuccess = true,
            StatusCode = 200
        };
    }*/
}