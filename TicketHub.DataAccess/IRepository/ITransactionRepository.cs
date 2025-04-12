using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<List<Transaction>> GetTransactionsAsync(DateTime startDate, DateTime endDate);
}