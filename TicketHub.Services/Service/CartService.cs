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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

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
        {
            return new ResponseDto()
            {
                Message = "You are not authorized to view all carts",
                IsSuccess = false,
                StatusCode = 403
            };
        }

        // Lấy tất cả `Cart`
        var allCarts = await _unitOfWork.CartRepository.GetAllAsync(includeProperties: "CartItems");

        if (!allCarts.Any())
        {
            return new ResponseDto()
            {
                Message = "No carts found",
                IsSuccess = false,
                StatusCode = 404
            };
        }

        // Tính tổng số trang (TotalPages)
        int totalCarts = allCarts.Count();
        int totalPages = (int)Math.Ceiling((double)totalCarts / pageSize);

        // Lấy dữ liệu theo trang (Pagination)
        var paginatedCarts = allCarts
            .OrderByDescending(c => c.CartId) // Sắp xếp theo Cart mới nhất
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Chuyển đổi dữ liệu
        var cartDtos = paginatedCarts.Select(c => new
        {
            CartId = c.CartId,
            UserId = c.UserId, // Chủ sở hữu giỏ hàng
            TotalAmount = c.TotalAmount, // Tổng tiền giỏ hàng
            TotalTickets = c.CartItems.Count // Số lượng vé trong giỏ hàng
        }).ToList();

        return new ResponseDto()
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

    //admin
    public async Task<ResponseDto> GetCartByUserId(ClaimsPrincipal User, string userId)
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
    }

    public async Task<ResponseDto> GetAllCartItem(ClaimsPrincipal User)
    {
        try
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return new ResponseDto
                {
                    Message = "User not found",
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            // Lấy giỏ hàng của User
            var cart = await _unitOfWork.CartRepository.GetAsync(x => x.UserId == userId,
                includeProperties: "CartItems.Ticket");

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                return new ResponseDto
                {
                    Message = "Cart is empty",
                    IsSuccess = true,
                    StatusCode = 200,
                    Result = new List<object>() // Trả về danh sách rỗng nếu không có cart items
                };
            }

            // Chuyển đổi danh sách CartItems thành DTO để trả về
            var cartItemsDto = cart.CartItems.Select(ci => new
            {
                CartItemId = ci.CartItemId,
                TicketId = ci.Ticket.TicketId,
                TicketName = ci.Ticket.TicketName,
                TicketPrice = ci.Ticket.TicketPrice
            }).ToList();

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

    public async Task<ResponseDto> AddToCart(ClaimsPrincipal User, AddToCartDTO addToCartDto)
    {
        try
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return new ResponseDto()
                {
                    Message = "User was not found",
                    IsSuccess = false,
                    StatusCode = 404,
                    Result = null
                };
            }

            var ticket = await _unitOfWork.TicketRepository.GetAsync(x => x.TicketId == addToCartDto.TicketId);
            if (ticket == null)
            {
                return new ResponseDto()
                {
                    Message = "Ticket was not found",
                    IsSuccess = false,
                    StatusCode = 404,
                    Result = null
                };
            }

            if (ticket.UserId == userId)
            {
                return new ResponseDto
                {
                    Message = "You cannot purchase your own ticket.",
                    IsSuccess = false,
                    StatusCode = 400,
                    Result = null
                };
            }

            // Lấy giỏ hàng của người dùng  
            var cart = await _unitOfWork.CartRepository.GetAsync(x => x.UserId == userId);

            // Nếu giỏ hàng không tồn tại thì tạo giỏ hàng mới  
            if (cart == null)
            {
                cart = new Cart()
                {
                    CartId = Guid.NewGuid(),
                    UserId = userId,
                    TotalAmount = 0
                };
                await _unitOfWork.CartRepository.AddAsync(cart);
                await _unitOfWork.SaveAsync();
            }

            // Kiểm tra xem vé đã có trong giỏ hay chưa  
            var cartItem = await _unitOfWork.CartItemRepository.GetAsync(x =>
                x.TicketId == ticket.TicketId && x.CartId == cart.CartId);
            if (cartItem != null)
            {
                return new ResponseDto()
                {
                    IsSuccess = true,
                    Result = null,
                    StatusCode = 200,
                    Message = "A ticket already exists in the cart."
                };
            }

            // Cập nhật tổng số tiền trong giỏ hàng  
            cart.TotalAmount += ticket.TicketPrice;
            _unitOfWork.CartRepository.Update(cart);

            // Tạo và thêm item vào giỏ hàng  
            var newCartItem = new CartItem()
            {
                CartId = cart.CartId,
                CartItemId = Guid.NewGuid(),
                TicketId = ticket.TicketId,
                Status = "1"
            };

            await _unitOfWork.CartItemRepository.AddAsync(newCartItem);
            await _unitOfWork.SaveAsync();

            // Chuyển đổi `cart` sang `CartDto` để tránh vòng lặp khi tuần tự hóa
            var cartDto = new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                TotalAmount = cart.TotalAmount,
                CartItemsDtos = cart.CartItems?.Select(item => new CartItemDto
                {
                    CartItemId = item.CartItemId,
                    CartId = cart.CartId,
                    TicketId = item.TicketId,
                    TicketPrice = ticket.TicketPrice,
                    Status = item.Status
                }).ToList()!
            };

            return new ResponseDto
            {
                Message = "Ticket added to cart successfully.",
                IsSuccess = true,
                StatusCode = 200,
                Result = cartDto
            };
        }
        catch (Exception e)
        {
            return new ResponseDto()
            {
                Message = "An error occurred while adding the ticket to the cart: " + e.Message,
                IsSuccess = false,
                StatusCode = 500,
                Result = null
            };
        }
    }

    public async Task<ResponseDto> RemoveFromCart(ClaimsPrincipal User, Guid TicketId)
    {
        var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return new ResponseDto()
            {
                Message = "User was not found",
                IsSuccess = false,
                StatusCode = 404,
                Result = null
            };
        }

        var cart = await _unitOfWork.CartRepository.GetAsync(x => x.UserId == userId,
            includeProperties: "CartItems.Ticket");
        if (cart == null || !cart.CartItems.Any())
        {
            return new ResponseDto()
            {
                Message = "Cart is empty",
                IsSuccess = false,
                StatusCode = 404,
                Result = null
            };
        }

        var cartItem = cart.CartItems.FirstOrDefault(x => x.TicketId == TicketId);
        if (cartItem == null)
        {
            return new ResponseDto()
            {
                Message = "Ticket was not found in the cart",
                IsSuccess = false,
                StatusCode = 404,
                Result = null
            };
        }

        // Xóa vé khỏi giỏ hàng
        _unitOfWork.CartItemRepository.Remove(cartItem);
        await _unitOfWork.SaveAsync();

        return new ResponseDto()
        {
            Message = "Ticket removed from cart successfully",
            IsSuccess = true,
            StatusCode = 200,
            Result = null
        };
    }

    public async Task<ResponseDto> Checkout(ClaimsPrincipal User, CheckoutDto checkoutDto)
    {
        try
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return new ResponseDto
                {
                    Message = "User not found",
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            // Lấy giỏ hàng của user dựa vào UserId
            var cartList = await _unitOfWork.CartRepository.GetAllAsync(x => x.UserId == userId);
            var cart = cartList.FirstOrDefault(); // Giả sử mỗi user chỉ có một Cart

            if (cart == null)
            {
                return new ResponseDto
                {
                    Message = "Cart not found.",
                    IsSuccess = false,
                    StatusCode = 404
                };
            }

            // Lấy toàn bộ CartItem trong giỏ hàng (dựa vào CartId)
            var allCartItems = await _unitOfWork.CartItemRepository
                .GetAllAsync(x => x.CartId == cart.CartId, includeProperties: "Ticket");

            // Tính tổng tiền của giỏ hàng trước khi checkout
            double totalCartPriceBefore = allCartItems.Sum(ci => ci.Ticket.TicketPrice);

            // Lọc danh sách CartItem cần cập nhật trạng thái dựa trên CartItemId gửi lên
            var cartItemsToUpdate = allCartItems
                .Where(ci => checkoutDto.CartItemIds.Contains(ci.CartItemId))
                .ToList();

            if (!cartItemsToUpdate.Any())
            {
                return new ResponseDto
                {
                    Message = "No valid cart items selected for checkout.",
                    IsSuccess = false,
                    StatusCode = 400
                };
            }

            // Lấy danh sách CartItem đã được cập nhật, bao gồm thông tin cần thiết
            var updatedCartItems = cartItemsToUpdate.Select(ci => new
            {
                CartItemId = ci.CartItemId,
                TicketId = ci.Ticket.TicketId,
                TicketName = ci.Ticket.TicketName,
                Price = ci.Ticket.TicketPrice
            }).ToList();

            // Tính tổng tiền của các CartItem được ẩn (Status = 0)
            double totalRemovedPrice = updatedCartItems.Sum(ci => ci.Price);

            // Tính tổng tiền giỏ hàng sau khi checkout
            double totalCartPriceAfter = totalCartPriceBefore - totalRemovedPrice;

            // Cập nhật lại totalPrice trong Cart
            cart.TotalAmount = totalCartPriceAfter;
            _unitOfWork.CartRepository.Update(cart);

            // Đổi `Status` của các CartItem thành `0` (ẩn chúng)
            foreach (var cartItem in cartItemsToUpdate)
            {
                cartItem.Status = "0";
                _unitOfWork.CartItemRepository.Update(cartItem);
            }

            await _unitOfWork.SaveAsync();

            return new ResponseDto
            {
                Message = "Checkout successful. Cart items updated.",
                IsSuccess = true,
                StatusCode = 200,
                Result = new
                {
                    TotalPriceBefore = totalCartPriceBefore,
                    TotalPriceCartItemRemove = totalRemovedPrice,
                    TotalPriceAfter = totalCartPriceAfter,
                    UpdatedCartItems = updatedCartItems
                }
            };
        }
        catch (Exception e)
        {
            return new ResponseDto
            {
                Message = "An error occurred during checkout: " + e.Message,
                IsSuccess = false,
                StatusCode = 500
            };
        }
    }
}