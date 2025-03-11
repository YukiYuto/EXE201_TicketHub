namespace TicketHub.Models.DTO.Transaction;

public class GetAllTransactionByCustomerIdDto
{
    public Guid Transaction { get; set; }
    public Guid? OrderId { get; set; }
    public double Amount { get; set; }
    public DateTime TransactionDateTime { get; set; }
    public string TransactionMethod { get; set; }
    public string Status { get; set; }
}