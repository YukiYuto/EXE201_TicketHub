using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class Cart
{
    [Key] public Guid CartId { get; set; }
    public Guid CustomerId { get; set; }
    [ForeignKey("CustomerId")] public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}