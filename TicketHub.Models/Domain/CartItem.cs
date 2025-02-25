using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class CartItem
{
    [Key]
    public Guid CartItemId { get; set; }

    public Guid TicketId { get; set; }
    public Guid CartId { get; set; }

    [ForeignKey("CartId")]
    public virtual Cart Cart { get; set; } = null!;

    [ForeignKey("TicketId")]
    public virtual Ticket Ticket { get; set; } = null!; 
    public string Status { get; set; } = null!;
    
}