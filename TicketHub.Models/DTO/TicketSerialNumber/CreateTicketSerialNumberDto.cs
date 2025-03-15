namespace TicketHub.Models.DTO.TicketSerialNumber;

public class CreateTicketSerialNumberDto
{
    public Guid TicketTemplateId { get; set; }
    public string SerialNumber { get; set; } = null!;
}