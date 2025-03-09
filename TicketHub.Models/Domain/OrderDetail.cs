using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class OrderDetail
{
    [Key] public Guid OrderDetailId { get; set; }

    public Guid OrderId { get; set; }
    public Guid TicketTemplateId { get; set; }
    public Guid? TicketId { get; set; }
    public int Quantity { get; set; }

    [ForeignKey("TicketTemplateId")] public virtual TicketTemplate TicketTemplate { get; set; } = null!;
    [ForeignKey("TicketId")] public virtual Ticket? Ticket { get; set; }
    [ForeignKey("OrderId")] public virtual Orders Orders { get; set; } = null!;
}