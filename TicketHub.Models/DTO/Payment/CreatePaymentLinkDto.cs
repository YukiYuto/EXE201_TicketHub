namespace TicketHub.Models.DTO.Payment;

public class CreatePaymentLinkDto
{
    public long OrderNumber { get; set; }
    public string CustomerId { get; set; } = null!;
    public string CancelUrl { get; set; } = null!;
    public string ReturnUrl { get; set; } = null!;
}