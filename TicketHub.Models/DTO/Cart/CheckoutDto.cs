namespace TicketHub.Models.DTO;

public class CheckoutDto
{
    public List<Guid> TicketTemplateIds { get; set; } = new();
}