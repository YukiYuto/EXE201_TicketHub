using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class Organizer
{
    [Key] public Guid OrganizerId { get; set; }
    public string UserId { get; set; }
    [ForeignKey("UserId")] public virtual ApplicationUser User { get; set; }
    [StringLength(100)] public string? OrganizationName { get; set; }
    [StringLength(50)] public string? TaxId { get; set; }
}