using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class Transaction
{
    [Key] public Guid TransactionId { get; set; }
    public string CustomerId { get; set; } = null!;
    public Guid? OrderId { get; set; }
    public Guid PaymentId { get; set; }
    public double Amount { get; set; }
    public DateTime TransactionDateTime { get; set; }
    public string TransactionMethod { get; set; } = null!;

    [ForeignKey("CustomerId")] public virtual ApplicationUser ApplicationUser { get; set; } = null!;
    [ForeignKey("OrderId")] public virtual Orders Orders { get; set; }
    [ForeignKey("PaymentId")] public virtual Payment Payment { get; set; }
}