using System.ComponentModel.DataAnnotations;

namespace TicketHub.Models.DTO.Order;

public class CreateOrderDto
{
    [Required] public Guid CustomerId { get; set; }
    [Required] public Guid OrderId { get; set; }
    [Required] public long OrderNumber { get; set; }
    [Required] public double TotalPrice { get; set; }
}