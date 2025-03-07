using System.Security.Claims;
using TicketHub.Models.DTO;

namespace TicketHub.Services.IService;

public interface ICartService
{
    Task<ResponseDto> GetAllCarts(ClaimsPrincipal User, int pageNumber = 1, int pageSize = 10);

    Task<ResponseDto> AddToCart(ClaimsPrincipal User, AddToCartDTO addToCartDto);
    Task<ResponseDto> RemoveFromCart(ClaimsPrincipal User, Guid cartItemId);

    Task<ResponseDto> CheckoutCart(ClaimsPrincipal User, CheckoutDto checkoutDto);

    //Task<ResponseDto> GetCartByUserId(ClaimsPrincipal User, string userId);
    Task<ResponseDto> GetAllCartItem(ClaimsPrincipal User);
}