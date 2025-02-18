using System.ComponentModel.DataAnnotations;

namespace TicketHub.Models.DTO.Order;

public class GetOrderByIdDto
{
    public Guid OrderId { get; set; }
    [StringLength(450)] public string UserId { get; set; } = null!;
    public double TotalPrice { get; set; }
}