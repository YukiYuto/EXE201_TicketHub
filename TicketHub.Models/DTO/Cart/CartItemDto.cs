namespace TicketHub.Models.DTO;

public class CartItemDto
{
    public Guid CartItemId { get; set; } 
    public Guid CartId { get; set; } 
    public string Status { get; set; } = null!; 
    public Guid TicketId { get; set; }  
    public double TicketPrice { get; set; }
}