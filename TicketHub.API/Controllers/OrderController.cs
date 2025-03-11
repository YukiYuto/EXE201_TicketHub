using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketHub.Models.DTO;
using TicketHub.Services.IService;
using TicketHub.Utility.Constants;

namespace TicketHub.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [Authorize(Roles = StaticUserRoles.MemberManager)]
    public async Task<ActionResult<ResponseDto>> GetOders
    (
        [FromQuery] string? filterOn,
        [FromQuery] string? filterQuery,
        [FromQuery] string? sortBy,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var responseDto = await _orderService.GetOrders(User, filterOn, filterQuery, sortBy, pageNumber, pageSize);
        return StatusCode(responseDto.StatusCode, responseDto);
    }
/*
        [HttpGet("{orderId}")]
        public async Task<ActionResult<ResponseDto>> GetOrder
        (
            [FromRoute] Guid orderId
        )
        {
            var responseDto = await _orderService.GetOrder(User, orderId);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ResponseDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            var responseDto = await _orderService.CreateOrder(User, createOrderDto);
            return StatusCode(responseDto.StatusCode, responseDto);
        }

        [HttpDelete("{orderId}")]
        [Authorize]
        public async Task<ActionResult<ResponseDto>> DeleteOrder
        (
            [FromRoute] Guid orderId
        )
        {
            var responseDto = await _orderService.DeleteOrder(User, orderId);
            return StatusCode(responseDto.StatusCode, responseDto);
        }*/
}