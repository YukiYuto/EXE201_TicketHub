namespace TicketHub.Models.DTO;

public class CartDto
{
    public Guid CartId { get; set; }
    public Guid CustomerId { get; set; }
    public double TotalAmount { get; set; }
    public List<CartItemDto> CartItemsDtos { get; set; }
}