namespace TicketHub.Models.DTO.Payment;

public class ConfirmPayment
{
    public long orderNumber { get; set; }
    public Guid paymentTransactionId { get; set; }
}