using System.Security.Claims;
using AutoMapper;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO;
using TicketHub.Services.IService;

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

    public async Task<ResponseDto> GetCart(ClaimsPrincipal User)
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

        var cart = await _unitOfWork.CartRepository.GetAsync(x => x.UserId == userId);
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

        var cartItems =
            await _unitOfWork.CartItemRepository.GetAllAsync(x => x.CartId == cart.CartId, includeProperties: "Ticket");
        var cartDto = _mapper.Map<CartDto>(cart);
        cartDto.CartItemsDtos = _mapper.Map<List<CartItemDto>>(cartItems);

        return new ResponseDto()
        {
            Message = "Get Cart successfully",
            IsSuccess = true,
            StatusCode = 200,
            Result = cartDto
        };
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