using System.ComponentModel.DataAnnotations;

namespace TicketHub.Models.DTO;

public class AddToCartDTO
{
    [Required] public List<Guid> TicketIds { get; set; } = new List<Guid>();
}