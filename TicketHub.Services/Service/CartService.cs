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
            .OrderByDescending(c => c.CartId) // Sắp xếp theo Cart mới nhất
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Chuyển đổi dữ liệu
        var cartDtos = paginatedCarts.Select(c => new
        {
            c.CartId,
            // UserId = c.UserId, // Chủ sở hữu giỏ hàng
            // TotalAmount = c.TotalAmount, // Tổng tiền giỏ hàng
            TotalTickets = c.CartItems.Count // Số lượng vé trong giỏ hàng
        }).ToList();

        return new ResponseDto
        {
            Message = "Get all carts successfully",
            IsSuccess = true,
            StatusCode = 200,
            Result = new
            {
                TotalCarts = totalCarts, // Tổng số Cart
                TotalPages = totalPages, // Tổng số trang
                PageSize = pageSize, // Số Cart mỗi trang
                CurrentPage = pageNumber, // Trang hiện tại
                Carts = cartDtos // Danh sách Cart của trang hiện tại
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
                    Quantity = addToCartDto.Quantity
                };

                await _unitOfWork.CartItemRepository.AddAsync(cartItem);
            }

            await _unitOfWork.SaveAsync();
            return new ResponseDto
            {
                Message = "Ticket added to cart successfully.",
                IsSuccess = true,
                StatusCode = 200
                //Result = cartDto
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

        // 📌 Tìm giỏ hàng của user
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

        // 🔍 Tìm `CartItem` dựa trên `cartItemId`
        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);
        if (cartItem == null)
            return new ResponseDto
            {
                Message = "Cart item not found",
                IsSuccess = false,
                StatusCode = 404,
                Result = null
            };

        // 🆚 **Nếu là `TicketTemplate` → Giảm số lượng vé**
        if (cartItem.TicketTemplateId != Guid.Empty)
        {
            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
                _unitOfWork.CartItemRepository.Update(cartItem);
            }
            else
            {
                _unitOfWork.CartItemRepository.Remove(cartItem);
            }
        }
        else // 🆕 Nếu là `ResaleListing`, xóa luôn (chỉ có 1 vé)
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
                IsSuccess = false,
                Result = null
            };

        var customer = await _unitOfWork.CustomerRepository.GetAsync(c => c.UserId == userId);
        if (customer == null)
            return new ResponseDto
            {
                Message = "Customer not found",
                StatusCode = 404,
                IsSuccess = false,
                Result = null
            };

        // 🔹 Lấy giỏ hàng của khách hàng
        var cart = await _unitOfWork.CartRepository.GetAsync(
            c => c.CustomerId == customer.CustomerId,
            "CartItems.TicketTemplate"
        );

        if (cart == null || !cart.CartItems.Any())
            return new ResponseDto
            {
                Message = "Cart is empty",
                StatusCode = 400,
                IsSuccess = false,
                Result = null
            };

        // 🔹 Lọc ra những vé khách hàng muốn mua
        var selectedCartItems = cart.CartItems
            .Where(ci => checkoutCartDto.TicketTemplateIds.Contains(ci.TicketTemplate.TicketTemplateId))
            .ToList();

        if (!selectedCartItems.Any())
            return new ResponseDto
            {
                Message = "No valid tickets selected!",
                StatusCode = 400,
                IsSuccess = false,
                Result = null
            };

        // 🔹 Tạo đơn hàng mới
        var order = new Orders
        {
            OrderId = Guid.NewGuid(),
            CustomerId = customer.CustomerId,
            OrderNumber = DateTime.UtcNow.Ticks,
            TotalPrice = 0
        };

        var orderTickets = new List<OrderDetail>();
        var tickets = new List<Ticket>();

        foreach (var cartItem in selectedCartItems)
        {
            var ticketTemplate = cartItem.TicketTemplate;

            if (ticketTemplate.AvailableQuantity <= 0)
                return new ResponseDto
                {
                    Message = $"Ticket {ticketTemplate.TicketName} is sold out!",
                    StatusCode = 400,
                    IsSuccess = false,
                    Result = null
                };

            var ticket = new Ticket
            {
                TicketId = Guid.NewGuid(),
                TicketTemplateId = ticketTemplate.TicketTemplateId,
                CustomerId = customer.CustomerId,
                Status = TicketStatus.Success,
                IsVisible = true
            };

            tickets.Add(ticket);
            orderTickets.Add(new OrderDetail { OrderId = order.OrderId, TicketId = ticket.TicketId });

            ticketTemplate.AvailableQuantity -= 1;
            order.TotalPrice += ticketTemplate.TicketPrice;
        }

        // 🔹 Lưu tất cả dữ liệu vào database
        await _unitOfWork.OrderRepository.AddAsync(order);
        await _unitOfWork.TicketRepository.AddRangeAsync(tickets);
        await _unitOfWork.OrderDetailRepository.AddRangeAsync(orderTickets);
        _unitOfWork.TicketTemplateRepository.UpdateRange(selectedCartItems.Select(ci => ci.TicketTemplate));

        // 🔹 Xóa chỉ những vé đã mua khỏi giỏ hàng
        _unitOfWork.CartItemRepository.RemoveRange(selectedCartItems);
        await _unitOfWork.SaveAsync();

        return new ResponseDto
        {
            Message = "Checkout successfully!",
            StatusCode = 200,
            IsSuccess = true,
            Result = new { order.OrderId, order.TotalPrice }
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
                .Where(ci => ci.Status == "1") // Chỉ lấy các mục có status = "1"
                .Select(ci => new CartItemDto
                {
                    CartItemId = ci.CartItemId,
                    //TicketTemplateId = ci.TicketTemplate.TicketTemplateId,
                    //TicketName = ci.TicketTemplate.TicketName,
                    TicketPrice = ci.TicketTemplate.TicketPrice
                    //Quantity = ci.Quantity,
                    //Rank = ci.TicketTemplate.Rank,
                    // ImageTicket = ci.TicketTemplate.ImageTicket
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