namespace TicketHub.Models.DTO.Event;

public class UpdateEventDto
{
    public Guid EventId { get; set; }
    public string EventName { get; set; } = null!;
    public string EventDescription { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public string City { get; set; } = null!;
    public string District { get; set; } = null!;
    public string Address { get; set; } = null!;
}