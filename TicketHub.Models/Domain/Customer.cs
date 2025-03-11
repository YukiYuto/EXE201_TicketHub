using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class Customer
{
    [Key] public Guid CustomerId { get; set; }
    public string UserId { get; set; }
    [ForeignKey("UserId")] public virtual ApplicationUser User { get; set; }

    [StringLength(12)] public string? CCCD { get; set; }
    [StringLength(10)] public string? Gender { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();
    public virtual ICollection<ResaleListing> ResaleListings { get; set; } = new List<ResaleListing>();
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    [NotMapped] public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}