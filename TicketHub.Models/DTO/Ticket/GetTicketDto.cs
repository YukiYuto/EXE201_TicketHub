using TicketHub.Models.Domain;

namespace TicketHub.Models.DTO.Ticket;

public class GetTicketDto
{
    //Ticket
    public Guid TicketId { get; set; }
    public Guid TicketTemplateId { get; set; }
    public Guid CustomerId { get; set; }
    public string TicketDescription { get; set; } = null!;
    public bool IsFromExternal { get; set; }
    public bool IsVisible { get; set; }

    public Guid SerialNumberId { get; set; }

    //TicketTemplate
    public string TicketName { get; set; } = null!;
    public Guid EventId { get; set; }
    public string EventName { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public double TicketPrice { get; set; }
    public string? TicketImage { get; set; }
    public string SerialNumber { get; set; } = null!;
    public string Rank { get; set; } = null!;
    public TicketStatus Status { get; set; }
    public bool NegotiationStatus { get; set; }
    public DateTime EventDate { get; set; }
}