using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<Payment> GetPaymentByOrderNumber(long orderNumber)
    {
        return await _context.Payments.FirstOrDefaultAsync(x => x.OrderNumber == orderNumber);
    }

    public async Task<Payment> GetPaymentTransactionIdByOrderNumber(Guid paymentTransactionId)
    {
        return await _context.Payments.FirstOrDefaultAsync(x => x.PaymentTransactionId == paymentTransactionId);
    }

    public  void Update(Payment payment)
    {
        _context.Payments.Update(payment);
    }
}