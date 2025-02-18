using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class CartItem
{
    public Guid TicketId { get; set; }
    [ForeignKey("TicketId")] public virtual Ticket Ticket { get; set; } = null!;
    public Guid CartId { get; set; }
    [ForeignKey("CartId")] public virtual Cart Cart { get; set; } = null!;
}