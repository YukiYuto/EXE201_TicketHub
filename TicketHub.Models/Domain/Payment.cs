using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class Payment
{
    [Key]
    public Guid PaymentTransactionId { get; set; }
    public long? OrderNumber { get; set; }
    [ForeignKey("OrderNumber")] public virtual Orders? Orders { get; set; }
    public int Amount { get; set; }
    public string? Description { get; set; }
    public string? CancelUrl { get; set; }
    public string? ReturnUrl { get; set; }
    public long? ExpiredAt { get; set; }
    public string? Signature { get; set; }
    public DateTime? CreatedAt { get; set; }

    public string? Status { get; set; }
}