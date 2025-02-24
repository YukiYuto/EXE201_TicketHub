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

        var cartItems = await _unitOfWork.CartItemRepository.GetAllAsync(x => x.CartId == cart.CartId, includeProperties: "Ticket");
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

            // Lấy giỏ hàng của người dùng
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

            // Lưu danh sách các vé đã thêm
            List<CartItem> addedCartItems = new();

            foreach (var ticketId in addToCartDto.TicketIds)
            {
                var ticket = await _unitOfWork.TicketRepository.GetAsync(x => x.TicketId == ticketId);
                if (ticket == null)
                {
                    continue; // Bỏ qua nếu vé không tồn tại
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

                // Kiểm tra xem vé đã có trong giỏ chưa
                var cartItem =
                    await _unitOfWork.CartItemRepository.GetAsync(
                        x => x.TicketId == ticketId && x.CartId == cart.CartId);
                if (cartItem != null)
                {
                    continue; // Bỏ qua nếu vé đã tồn tại
                }

                // Thêm vé vào giỏ hàng
                cart.TotalAmount += ticket.TicketPrice;
                var newCartItem = new CartItem()
                {
                    CartId = cart.CartId,
                    TicketId = ticket.TicketId
                };
                addedCartItems.Add(newCartItem);
            }

            // Thêm tất cả vé vào giỏ hàng một lần
            if (addedCartItems.Any())
            {
                await _unitOfWork.CartItemRepository.AddRangeAsync(addedCartItems);
                _unitOfWork.CartRepository.Update(cart);
                await _unitOfWork.SaveAsync();
            }

            return new ResponseDto
            {
                Message = "Tickets added to cart successfully.",
                IsSuccess = true,
                StatusCode = 200,
                Result = addedCartItems.Select(x => x.TicketId).ToList()
            };
        }
        catch (Exception e)
        {
            return new ResponseDto()
            {
                Message = "An error occurred while adding tickets to the cart: " + e.Message,
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

        // Cập nhật tổng giá tiền giỏ hàng
        cart.TotalAmount -= cartItem.Ticket.TicketPrice;
        _unitOfWork.CartRepository.Update(cart);

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
}