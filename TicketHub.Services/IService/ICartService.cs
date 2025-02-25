using System.Security.Claims;
using TicketHub.Models.DTO;

namespace TicketHub.Services.IService;

public interface ICartService
{
    Task<ResponseDto> GetCart(ClaimsPrincipal User);
    Task<ResponseDto> AddToCart(ClaimsPrincipal User, AddToCartDTO addToCartDto);
    Task<ResponseDto> RemoveFromCart(ClaimsPrincipal User, Guid TicketId);
    Task<ResponseDto> Checkout(ClaimsPrincipal User, CheckoutDto checkoutDto);
    
}