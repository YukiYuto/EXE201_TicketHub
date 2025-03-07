using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class CartItem
{
    [Key] public Guid CartItemId { get; set; }

    public Guid TicketTemplateId { get; set; }
    public Guid CartId { get; set; }

    [ForeignKey("CartId")] public virtual Cart Cart { get; set; } = null!;

    [ForeignKey("TicketTemplateId")] public virtual TicketTemplate TicketTemplate { get; set; } = null!;
}