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
    public Guid? TicketTemplateId { get; set; }
    [ForeignKey("TicketTemplateId")] public virtual TicketTemplate TicketTemplate { get; set; }

    public Guid CustomerId { get; set; }
    [ForeignKey("CustomerId")] public virtual Customer Customer { get; set; } = null!;

    [StringLength(100)] public string TicketDescription { get; set; } = null!;

    public Guid? SerialNumberId { get; set; }
    [ForeignKey("SerialNumberId")] public virtual TicketSerialNumber TicketSerialNumber { get; set; }
    public bool IsFromExternal { get; set; } = false;
    public bool IsVisible { get; set; } = true;

    public TicketStatus Status { get; set; }
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ResaleListing> ResaleListings { get; set; } = new List<ResaleListing>();
}