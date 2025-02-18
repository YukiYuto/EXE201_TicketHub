using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class TicketTransfers
{
    [Key]public Guid TicketTransferId { get; set; }
    public Guid TicketId { get; set; }
    [ForeignKey("TicketId")] public virtual Ticket Ticket { get; set; } = null!;
    public string SellerId { get; set; } = null!;
    [ForeignKey("SellerId")] public virtual ApplicationUser Seller { get; set; } = null!;
    public string BuyerId { get; set; } = null!;
    [ForeignKey("BuyerId")] public virtual ApplicationUser Buyer { get; set; } = null!;
    public double Amount { get; set; }
}