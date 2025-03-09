using System.ComponentModel.DataAnnotations;

namespace TicketHub.Models.DTO.Event;

public class GetEventDto
{
    public Guid EventId { get; set; }
    [StringLength(100)] public string EventName { get; set; } = null!;
    [StringLength(500)] public string EventDescription { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = null!;
    public int Status { get; set; }
    public string? EventImage { get; set; }
}