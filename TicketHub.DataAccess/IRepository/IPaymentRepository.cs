using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment> GetPaymentByOrderNumber(long orderNumber);
    Task<Payment> GetPaymentTransactionIdByOrderNumber(Guid paymentTransactionId);
    void Update(Payment payment);
}