using System.Security.Claims;
using TicketHub.Models.DTO;

namespace TicketHub.Services.IService;

public interface ICartService
{
    Task<ResponseDto> GetAllCarts(ClaimsPrincipal User, int pageNumber = 1, int pageSize = 10);

    Task<ResponseDto> AddToCart(ClaimsPrincipal User, AddToCartDTO addToCartDto);
    /*Task<ResponseDto> GetCartByUserId(ClaimsPrincipal User, string userId);
    Task<ResponseDto> GetAllCartItem(ClaimsPrincipal User);

    Task<ResponseDto> RemoveFromCart(ClaimsPrincipal User, Guid TicketId);
    Task<ResponseDto> Checkout(ClaimsPrincipal User, CheckoutDto checkoutDto);*/
}