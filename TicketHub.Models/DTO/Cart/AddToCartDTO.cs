using System.ComponentModel.DataAnnotations;

namespace TicketHub.Models.DTO;

public class AddToCartDTO
{
    [Required] public Guid TicketTemplateId { get; set; }
    [Required] public int Quantity { get; set; }
}