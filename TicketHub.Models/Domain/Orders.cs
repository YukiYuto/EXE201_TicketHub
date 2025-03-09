using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class Orders : BaseEntity<string, string, string>
{
    [Key] public Guid OrderId { get; set; }
    public long OrderNumber { get; set; }
    public Guid CustomerId { get; set; }
    [ForeignKey("CustomerId")] public virtual Customer Customer { get; set; } = null!;
    public double TotalPrice { get; set; }
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}