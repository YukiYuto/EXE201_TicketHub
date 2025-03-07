namespace TicketHub.Models.DTO.TicketTemplate;

public class TicketTemplateDto
{
    public string TicketName { get; set; } = null!;
    public Guid EventId { get; set; }
    public string? ImageTicket { get; set; }
    public double TicketPrice { get; set; }
    public int TotalQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public string Rank { get; set; } = null!;
}