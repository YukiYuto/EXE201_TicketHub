using Microsoft.AspNetCore.Mvc;
using TicketHub.Models.DTO;
using TicketHub.Services.IService;

namespace TicketHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = StaticUserRoles.Member)]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        [Route("GetCart")]
        public async Task<IActionResult> GetCart()
        {
            var responseDto = await _cartService.GetCart(User);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpPost]
        [Route("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDTO addToCartDto)
        {
            var responseDto = await _cartService.AddToCart
            (
                User,
                addToCartDto
            );
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpDelete]
        [Route("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart([FromQuery] Guid ticketId)
        {
            var responseDto = await _cartService.RemoveFromCart
            (
                User,
                ticketId
            );
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpPost]
        [Route("Checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto checkoutDto)
        {
            var responseDto = await _cartService.Checkout(User, checkoutDto);
            return StatusCode(responseDto.StatusCode, responseDto);
        }
    }
}