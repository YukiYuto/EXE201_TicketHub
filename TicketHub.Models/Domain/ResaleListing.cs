using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class ResaleListing
{
    [Key]
    public Guid ListingId { get; set; }

    public Guid TicketId { get; set; }

    [ForeignKey("TicketId")]
    public virtual Ticket Ticket { get; set; } = null!;

    public string SellerId { get; set; } = null!;

    [ForeignKey("SellerId")]
    public virtual ApplicationUser Seller { get; set; } = null!;

    public double Price { get; set; }  // Giá mà người bán đặt

    public bool IsNegotiable { get; set; } = true; // Có thể thương lượng hay không

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Ngày tạo listing

    public bool IsSold { get; set; } = false; // Vé đã được mua chưa?
}