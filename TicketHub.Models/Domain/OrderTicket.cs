using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class OrderTicket
{
    public Guid OrderId { get; set; }
    [ForeignKey("OrderId")] public virtual Orders Orders { get; set; } = null!;
    public Guid TicketId { get; set; }
    [ForeignKey("TicketId")] public virtual Ticket Ticket { get; set; } = null!;
}