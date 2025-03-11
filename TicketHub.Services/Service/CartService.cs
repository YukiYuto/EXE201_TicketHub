using System.Security.Claims;
using AutoMapper;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO;
using TicketHub.Services.IService;
using TicketHub.Utility.Constants;

namespace TicketHub.Services.Service;

public class CartService : ICartService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CartService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    //admin
    public async Task<ResponseDto> GetAllCarts(ClaimsPrincipal User, int pageNumber = 1, int pageSize = 10)
    {
        // Kiểm tra quyền admin
        if (!User.IsInRole(StaticUserRoles.Admin))
            return new ResponseDto
            {
                Message = "You are not authorized to view all carts",
                IsSuccess = false,
                StatusCode = 403
            };

        // Lấy tất cả `Cart`
        var allCarts = await _unitOfWork.CartRepository.GetAllAsync(includeProperties: "CartItems");

        if (!allCarts.Any())
            return new ResponseDto
            {
                Message = "No carts found",
                IsSuccess = false,
                StatusCode = 404
            };

        // Tính tổng số trang (TotalPages)
        var totalCarts = allCarts.Count();
        var totalPages = (int)Math.Ceiling((double)totalCarts / pageSize);

        // Lấy dữ liệu theo trang (Pagination)
        var paginatedCarts = allCarts
            .OrderByDescending(c => c.CartId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Chuyển đổi dữ liệu
        var cartDtos = paginatedCarts.Select(c => new
        {
            c.CartId,
            TotalTickets = c.CartItems.Count
        }).ToList();

        return new ResponseDto
        {
            Message = "Get all carts successfully",
            IsSuccess = true,
            StatusCode = 200,
            Result = new
            {
                TotalCarts = totalCarts,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Carts = cartDtos
            }
        };
    }

    public async Task<ResponseDto> AddToCart(ClaimsPrincipal User, AddToCartDTO addToCartDto)
    {
        try
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return new ResponseDto
                {
                    Message = "User was not found",
                    IsSuccess = false,
                    StatusCode = 404,
                    Result = null
                };

            var customer = await _unitOfWork.CustomerRepository.GetAsync(c => c.UserId == userId);
            if (customer == null)
                return new ResponseDto
                {
                    Message = "Customer was not found",
                    IsSuccess = false,
                    StatusCode = 404,
                    Result = null
                };

            var ticketTemplate = await _unitOfWork.TicketTemplateRepository
                .GetAsync(t => t.TicketTemplateId == addToCartDto.TicketTemplateId);
            if (ticketTemplate == null)
                return new ResponseDto
                {
                    Message = "Ticket template not found",
                    IsSuccess = false,
                    StatusCode = 404,
                    Result = null
                };

            if (ticketTemplate.AvailableQuantity < addToCartDto.Quantity)
                return new ResponseDto
                {
                    Message = "Not enough tickets available",
                    IsSuccess = false,
                    StatusCode = 400,
                    Result = null
                };

            // Lấy giỏ hàng của người dùng
            var cart = await _unitOfWork.CartRepository.GetAsync(x => x.CustomerId == customer.CustomerId);

            // Nếu giỏ hàng không tồn tại thì tạo giỏ hàng mới
            if (cart == null)
            {
                cart = new Cart
                {
                    CartId = Guid.NewGuid(),
                    CustomerId = customer.CustomerId
                };
                await _unitOfWork.CartRepository.AddAsync(cart);
                await _unitOfWork.SaveAsync();
            }

            var cartItem = await _unitOfWork.CartItemRepository.GetAsync(ci =>
                ci.CartId == cart.CartId && ci.TicketTemplateId == ticketTemplate.TicketTemplateId);

            if (cartItem != null)
            {
                cartItem.Quantity += addToCartDto.Quantity;
                _unitOfWork.CartItemRepository.Update(cartItem);
            }
            else
            {
                cartItem = new CartItem
                {
                    CartItemId = Guid.NewGuid(),
                    CartId = cart.CartId,
                    TicketTemplateId = ticketTemplate.TicketTemplateId,
                    Quantity = addToCartDto.Quantity,
                    Status = "1"
                };

                await _unitOfWork.CartItemRepository.AddAsync(cartItem);
            }

            await _unitOfWork.SaveAsync();

            var cartItemDto = new GetCartItem
            {
                CartItemId = cartItem.CartItemId,
                CartId = cartItem.CartId,
                TicketTemplateId = cartItem.TicketTemplateId,
                Quantity = cartItem.Quantity,
                Status = cartItem.Status,
                TicketTemplateName = ticketTemplate.TicketName,
                TicketPrice = ticketTemplate.TicketPrice,
                ImageUrl = ticketTemplate.ImageTicket,
                Rank = ticketTemplate.Rank
            };

            return new ResponseDto
            {
                Message = "Ticket added to cart successfully.",
                IsSuccess = true,
                StatusCode = 200,
                Result = cartItemDto
            };
        }
        catch (Exception e)
        {
            return new ResponseDto
            {
                Message = "An error occurred while adding the ticket to the cart: " + e.Message,
                IsSuccess = false,
                StatusCode = 500,
                Result = null
            };
        }
    }

    public async Task<ResponseDto> RemoveFromCart(ClaimsPrincipal User, Guid cartItemId)
    {
        var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return new ResponseDto
            {
                Message = "User was not found",
                IsSuccess = false,
                StatusCode = 404,
                Result = null
            };

        var customer = await _unitOfWork.CustomerRepository.GetAsync(c => c.UserId == userId);
        if (customer == null)
            return new ResponseDto
            {
                Message = "Customer not found",
                IsSuccess = false,
                StatusCode = 404,
                Result = null
            };

        var cart = await _unitOfWork.CartRepository.GetAsync(x => x.CustomerId == customer.CustomerId,
            "CartItems.TicketTemplate");
        if (cart == null || !cart.CartItems.Any())
            return new ResponseDto
            {
                Message = "Cart is empty",
                IsSuccess = false,
                StatusCode = 404,
                Result = null
            };

        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);
        if (cartItem == null)
            return new ResponseDto
            {
                Message = "Cart item not found",
                IsSuccess = false,
                StatusCode = 404,
                Result = null
            };

        if (cartItem.TicketTemplateId != Guid.Empty)
        {
            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity -= 1;
                _unitOfWork.CartItemRepository.Update(cartItem);
            }
            else
            {
                _unitOfWork.CartItemRepository.Remove(cartItem);
            }
        }
        else
        {
            _unitOfWork.CartItemRepository.Remove(cartItem);
        }

        await _unitOfWork.SaveAsync();

        return new ResponseDto
        {
            Message = "Ticket removed from cart successfully",
            IsSuccess = true,
            StatusCode = 200,
            Result = null
        };
    }

    public async Task<ResponseDto> CheckoutCart(ClaimsPrincipal user, CheckoutDto checkoutCartDto)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return new ResponseDto
            {
                Message = "User not found",
                StatusCode = 404,
                IsSuccess = false
            };

        var customer = await _unitOfWork.CustomerRepository.GetAsync(c => c.UserId == userId);
        if (customer == null)
            return new ResponseDto
            {
                Message = "Customer not found",
                StatusCode = 404,
                IsSuccess = false
            };

        var cart = await _unitOfWork.CartRepository.GetAsync(
            c => c.CustomerId == customer.CustomerId,
            "CartItems.TicketTemplate"
        );

        if (cart == null || !cart.CartItems.Any())
            return new ResponseDto
            {
                Message = "Cart is empty",
                StatusCode = 400,
                IsSuccess = false
            };

        var selectedCartItems = cart.CartItems
            .Where(ci => checkoutCartDto.TicketTemplateIds.Contains(ci.TicketTemplate.TicketTemplateId))
            .ToList();

        if (!selectedCartItems.Any())
            return new ResponseDto { Message = "No valid tickets selected!", StatusCode = 400, IsSuccess = false };

        // 🔹 Tạo đơn hàng (Chưa có Ticket)
        var order = new Orders
        {
            OrderId = Guid.NewGuid(),
            CustomerId = customer.CustomerId,
            OrderNumber = await _unitOfWork.OrderRepository.GenerateUniqueNumberAsync(),
            TotalPrice = selectedCartItems.Sum(ci => ci.Quantity * ci.TicketTemplate.TicketPrice),
            Status = "Pending"
        };

        // 🔹 Lưu OrderDetail (chỉ có TicketTemplateId, chưa có TicketId)
        var orderDetails = selectedCartItems.Select(cartItem => new OrderDetail
        {
            OrderDetailId = Guid.NewGuid(),
            OrderId = order.OrderId,
            TicketTemplateId = cartItem.TicketTemplate.TicketTemplateId,
            Quantity = cartItem.Quantity
        }).ToList();

        await _unitOfWork.OrderRepository.AddAsync(order);
        await _unitOfWork.OrderDetailRepository.AddRangeAsync(orderDetails);
        await _unitOfWork.SaveAsync();


        return new ResponseDto
        {
            Message = "Checkout successful! Please complete payment.",
            StatusCode = 200,
            IsSuccess = true,
            Result = new
            {
                order.OrderId,
                order.TotalPrice,
                order.OrderNumber
            }
        };
    }
    //admin
    /*public async Task<ResponseDto> GetCartByUserId(ClaimsPrincipal User, string userId)
    {
        // Kiểm tra quyền admin
        var isAdmin = User.IsInRole(StaticUserRoles.Admin);
        if (!isAdmin)
        {
            return new ResponseDto()
            {
                Message = "You are not authorized to view user carts",
                IsSuccess = false,
                StatusCode = 403
            };
        }

        // Lấy giỏ hàng của user
        var cart = await _unitOfWork.CartRepository.GetAsync(x => x.UserId == userId, includeProperties: "CartItems.Ticket");
        if (cart == null)
        {
            return new ResponseDto()
            {
                Message = "Cart not found for this user",
                IsSuccess = false,
                StatusCode = 404
            };
        }

        // Chuyển đổi sang DTO
        var cartDto = new CartDto
        {
            CartId = cart.CartId,
            UserId = cart.UserId,
            TotalAmount = cart.TotalAmount,
            CartItemsDtos = cart.CartItems.Select(ci => new CartItemDto
            {
                CartItemId = ci.CartItemId,
                TicketId = ci.Ticket.TicketId,
                TicketPrice = ci.Ticket.TicketPrice
            }).ToList()
        };

        return new ResponseDto()
        {
            Message = "Get cart successfully",
            IsSuccess = true,
            StatusCode = 200,
            Result = cartDto
        };
    }*/

    public async Task<ResponseDto> GetAllCartItem(ClaimsPrincipal user)
    {
        try
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return new ResponseDto
                {
                    Message = "User not found",
                    IsSuccess = false,
                    StatusCode = 404
                };

            var customer = await _unitOfWork.CustomerRepository.GetAsync(c => c.UserId == userId);
            if (customer == null)
                return new ResponseDto
                {
                    Message = "Customer not found",
                    IsSuccess = false,
                    StatusCode = 404
                };

            // Lấy giỏ hàng của User (bao gồm CartItems và TicketTemplate)
            var cart = await _unitOfWork.CartRepository.GetAsync(
                x => x.CustomerId == customer.CustomerId,
                "CartItems.TicketTemplate"
            );

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                return new ResponseDto
                {
                    Message = "Cart is empty",
                    IsSuccess = true,
                    StatusCode = 200,
                    Result = new List<object>() // Trả về danh sách rỗng nếu không có cart items
                };

            // Lọc các CartItem có Status == "1"
            var cartItemsDto = cart.CartItems
                .Where(ci => ci.Status == "1")
                .Select(ci => new CartItemDto
                {
                    CartItemId = ci.CartItemId,
                    TicketTemplateId = ci.TicketTemplate.TicketTemplateId,
                    TicketName = ci.TicketTemplate.TicketName,
                    TicketPrice = ci.TicketTemplate.TicketPrice,
                    Quantity = ci.Quantity,
                    Status = ci.Status,
                    Rank = ci.TicketTemplate.Rank,
                    ImageTicket = ci.TicketTemplate.ImageTicket
                })
                .ToList();

            return new ResponseDto
            {
                Message = "Retrieved cart items successfully",
                IsSuccess = true,
                StatusCode = 200,
                Result = cartItemsDto
            };
        }
        catch (Exception e)
        {
            return new ResponseDto
            {
                Message = "An error occurred: " + e.Message,
                IsSuccess = false,
                StatusCode = 500
            };
        }
    }
}