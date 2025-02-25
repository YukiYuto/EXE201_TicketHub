namespace TicketHub.Models.DTO;

public class CheckoutDto
{
    public List<Guid> CartItemIds { get; set; } = new();
}