namespace TicketHub.Models.DTO.Event;

public class CreateEventDto
{
    public string EventName { get; set; } = null!;
    public string EventDescription { get; set; } = null!;
    public DateTime EventDate { get; set; }

    public Guid CategoryId { get; set; }

    public string Location { get; set; } = null!;

    public string? EventImage { get; set; }
}