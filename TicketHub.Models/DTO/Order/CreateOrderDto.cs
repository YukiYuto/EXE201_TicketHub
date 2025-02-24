using System.ComponentModel.DataAnnotations;

namespace TicketHub.Models.DTO.Order;

public class CreateOrderDto
{
    [StringLength(450)] public string UserId { get; set; } = null!;
    public double TotalPrice { get; set; }
}