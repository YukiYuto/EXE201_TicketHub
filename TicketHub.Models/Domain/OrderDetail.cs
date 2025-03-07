using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class OrderDetail
{
    [Key] public Guid OrderDetailId { get; set; }

    public Guid OrderId { get; set; }
    [ForeignKey("OrderId")] public virtual Orders Orders { get; set; } = null!;
    public Guid TicketId { get; set; }
    [ForeignKey("TicketId")] public virtual Ticket Ticket { get; set; } = null!;
}