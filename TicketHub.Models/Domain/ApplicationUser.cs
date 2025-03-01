using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TicketHub.Models.Domain;

public class ApplicationUser : IdentityUser
{
    [StringLength(100)] public string? FullName { get; set; }
    public DateTime? BirthDate { get; set; }

    [StringLength(500)] public string? AvatarUrl { get; set; }

    [StringLength(50)] public string? Country { get; set; }

    [StringLength(12)] public string? CCCD { get; set; }

    [StringLength(100)] public string? Address { get; set; }

    // Các thuộc tính dành cho nhà tổ chức sự kiện
    [StringLength(100)] public string? OrganizationName { get; set; }

    [StringLength(50)] public string? TaxId { get; set; }

    // Thuộc tính xác định loại người dùng: "Customer", "Organizer", "SecondarySeller"
    [StringLength(20)] public string? UserType { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}