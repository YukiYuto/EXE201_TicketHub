using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketHub.Models.DTO;
using TicketHub.Services.IService;
using TicketHub.Utility.Constants;

namespace TicketHub.API.Controllers;

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

    [HttpGet("admin/all-carts")]
    [Authorize(Roles = StaticUserRoles.Admin)]
    public async Task<ActionResult<ResponseDto>> GetAllCarts(int pageNumber = 1, int pageSize = 10)
    {
        var responseDto = await _cartService.GetAllCarts(User, pageNumber, pageSize);
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

    /*[HttpGet("admin/cart-by-user")]
    [Authorize(Roles = StaticUserRoles.Admin)]
    public async Task<ActionResult<ResponseDto>> GetCartByUserId(string userId)
    {
        var responseDto = await _cartService.GetCartByUserId(User, userId);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpGet]
    [Authorize(Roles = StaticUserRoles.Member)]
    [Route("GetCartItem")]
    public async Task<IActionResult> GetCartItem()
    {
        var responseDto = await _cartService.GetAllCartItem(User);
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
    }*/
}