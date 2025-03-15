using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class TicketSerialNumber : BaseEntity<string, string, string>
{
    [Key] public Guid SerialNumberId { get; set; }
    public Guid? TicketTemplateId { get; set; }

    [StringLength(100)] public string SerialNumber { get; set; } = null!;

    [ForeignKey("TicketTemplateId")] public virtual TicketTemplate TicketTemplate { get; set; } = null!;
}