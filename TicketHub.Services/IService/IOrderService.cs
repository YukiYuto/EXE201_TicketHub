using System.Security.Claims;
using TicketHub.Models.DTO;

namespace TicketHub.Services.IService;

public interface IOrderService
{
    Task<ResponseDto> GetOrders
    (
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 0,
        int pageSize = 0
    );

    /* Task<ResponseDto> GetOrder(ClaimsPrincipal user, Guid orderId);

     Task<ResponseDto> CreateOrder(ClaimsPrincipal user, CreateOrderDto createOrderDto);
     Task<ResponseDto> UpdateOrder(ClaimsPrincipal user, UpdateOrderDto updateOrderDto);
     Task<ResponseDto> DeleteOrder(ClaimsPrincipal user, Guid orderId);*/
}