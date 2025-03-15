namespace TicketHub.Models.DTO.TicketSerialNumber;

public class GetTicketSerialNumberDto
{
    public Guid SerialNumberId { get; set; }
    public Guid? TicketTemplateId { get; set; }
    public string SerialNumber { get; set; } = null!;
}