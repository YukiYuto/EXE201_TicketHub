using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public enum TicketStatus
{
    Success = 0,
    Processing = 1,
    Rejected = 2
}

public class Ticket
{
    [Key] public Guid TicketId { get; set; }
    [StringLength(500)] public string TicketName { get; set; } = null!;
    public Guid EventId { get; set; }
    [ForeignKey("EventId")] public virtual Event Event { get; set; } = null!;
    [StringLength(450)] public string UserId { get; set; } = null!;
    [ForeignKey("UserId")] public virtual ApplicationUser ApplicationUser { get; set; } = null!;
    public Guid CategoryId { get; set; }
    [ForeignKey("CategoryId")] public virtual Category Category { get; set; } = null!;
    public double TicketPrice { get; set; }
    public double? NewPrice { get; set; }
    [StringLength(500)] public string TicketDescription { get; set; } = null!;
    [StringLength(20)] public string SerialNumber { get; set; } = null!;
    [StringLength(1000)] public string? TicketImage { get; set; }
    public bool NegotiationStatus { get; set; }

    public bool IsVisible { get; set; } = true;
    public TicketStatus Status { get; set; }
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<OrderTicket> OrderTickets { get; set; } = new List<OrderTicket>();
}