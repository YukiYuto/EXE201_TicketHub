using System.ComponentModel.DataAnnotations;

namespace TicketHub.Models.DTO;

public class AddToCartDTO
{
    [Required] public Guid TicketId { get; set; }
}