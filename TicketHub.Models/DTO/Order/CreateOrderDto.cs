using System.ComponentModel.DataAnnotations;

namespace TicketHub.Models.DTO.Order;

public class CreateOrderDto
{
    public List<Guid> CheckedOutCartItemIds { get; set; } = new();
    public double CheckoutTotalPrice { get; set; }
}