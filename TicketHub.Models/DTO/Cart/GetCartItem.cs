namespace TicketHub.Models.DTO;

public class GetCartItem
{
    public Guid CartItemId { get; set; }
    public Guid CartId { get; set; }
    public Guid TicketTemplateId { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; } = null!;
    public string TicketTemplateName { get; set; } = null!;
    public double TicketPrice { get; set; }
    public string? ImageUrl { get; set; }
}