using TicketHub.Models.Domain;

namespace TicketHub.Models.DTO.Ticket;

public class CreateTicketDto
{
    public string TicketName { get; set; } = null!;
    public string TicketDescription { get; set; } = null!;
    public Guid EventId { get; set; }
    public Guid CategoryId { get; set; }
    public double TicketPrice { get; set; }
    public string? TicketImage { get; set; }
    public string SerialNumber { get; set; } = null!;
    public TicketStatus Status { get; set; }
}