using Microsoft.EntityFrameworkCore;
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

    public async Task<List<Transaction>> GetTransactionsAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Transactions
            .Include(t => t.Customer)
            .ThenInclude(c => c.User)
            .Where(t => t.TransactionDateTime >= startDate.ToUniversalTime() &&
                        t.TransactionDateTime <= endDate.ToUniversalTime())
            .ToListAsync();
    }
}