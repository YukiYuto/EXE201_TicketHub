using System.Linq.Expressions;
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
    
}