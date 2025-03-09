namespace TicketHub.Models.DTO;

public class CartItemDto
{
    public Guid CartItemId { get; set; }
    public Guid TicketTemplateId { get; set; }
    public string TicketName { get; set; } = null!;
    public Guid TicketId { get; set; }
    public int Quantity { get; set; }
    public string Rank { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string ImageTicket { get; set; } = null!;
    public double TicketPrice { get; set; }
}