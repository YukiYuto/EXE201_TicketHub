namespace TicketHub.Models.DTO.Ticket;

public class TicketScanRequest
{
    public Guid TicketId { get; set; }
    public string SerialNumber { get; set; }
}