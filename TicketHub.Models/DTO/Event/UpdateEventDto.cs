using TicketHub.Models.DTO.TicketTemplate;

namespace TicketHub.Models.DTO.Event;

public class UpdateEventDto
{
    public Guid EventId { get; set; }
    public string EventName { get; set; } = null!;
    public string EventDescription { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = null!;
    public string? EventImage { get; set; }
    /*
    public List<TicketTemplateDto> TicketTemplates { get; set; } = new();
*/
}