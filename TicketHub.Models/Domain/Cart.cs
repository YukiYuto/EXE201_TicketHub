using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class Cart
{
    [Key] public Guid CartId { get; set; }
    public string UserId { get; set; } = null!;
    [ForeignKey("UserId")]public virtual ApplicationUser ApplicationUser { get; set; } = null!;
    public double TotalAmount { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}