namespace TicketHub.Models.DTO.Ticket;

public class UpdateTicketDto
{
    public Guid TicketTemplateId { get; set; }
    public string? TicketName { get; set; } = null!;
    public Guid? EventId { get; set; }
    public double? TicketPrice { get; set; }
    public string? TicketImage { get; set; }
    public int? TotalQuantity { get; set; }

    public string? Rank { get; set; }
    public bool? IsVisible { get; set; } = true;

    //public string SerialNumber { get; set; } = null!;
}