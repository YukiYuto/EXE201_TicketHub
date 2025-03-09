using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class Transaction
{
    [Key] public Guid TransactionId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid PaymentId { get; set; }
    public double Amount { get; set; }
    public DateTime TransactionDateTime { get; set; }
    public string TransactionMethod { get; set; } = null!;

    public string Status { get; set; } = null!;

    [ForeignKey("CustomerId")] public virtual Customer Customer { get; set; } = null!;
    [ForeignKey("OrderId")] public virtual Orders Orders { get; set; }
    [ForeignKey("PaymentId")] public virtual Payment Payment { get; set; }
}