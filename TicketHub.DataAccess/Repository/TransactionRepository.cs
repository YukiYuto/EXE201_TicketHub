using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    private readonly ApplicationDbContext _context;
    
    public TransactionRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}