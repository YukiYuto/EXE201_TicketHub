using System.ComponentModel.DataAnnotations;

namespace TicketHub.Models.DTO.Order;

public class GetOrderDto
{
    public Guid OrderId { get; set; }
    [StringLength(450)] public Guid CustomerId { get; set; }
    public double TotalPrice { get; set; }
    public long OrderNumber { get; set; }
}