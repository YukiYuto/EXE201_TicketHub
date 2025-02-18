using TicketHub.Models.Domain;

namespace TicketHub.Models.DTO.Ticket;

public class GetTicketDto
{
    public Guid TicketId { get; set; }
    public string TicketName { get; set; } = null!;
    public Guid EventId { get; set; }
    public string EventName { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public double TicketPrice { get; set; }
    public double? NewPrice { get; set; }
    public string? TicketImage { get; set; }
    public string TicketDescription { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public TicketStatus Status { get; set; }
    public bool IsVisible { get; set; }
    public bool NegotiationStatus { get; set; }


    public DateTime EventDate { get; set; }

    public string City { get; set; } = null!;
    public string District { get; set; } = null!;
    public string Address { get; set; } = null!;
}