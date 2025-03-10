namespace TicketHub.Models.DTO.Payment;

public class PaymentDto
{
    public Guid PaymentTransactionId { get; set; }
    public long? OrderNumber { get; set; }
    public int Amount { get; set; }
    public string? Description { get; set; }
    public string? CancelUrl { get; set; }
    public string? ReturnUrl { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Status { get; set; }
}