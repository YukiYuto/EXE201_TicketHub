using System.Linq.Expressions;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface ITicketRepository : IRepository<Ticket>
{
    void Update(Ticket ticket);
    void UpdateRange(IEnumerable<Ticket> tickets);
    Task<Ticket> GeTicketById(Guid ticketId);
    Task<IEnumerable<Ticket>> GetAllWithEventAndLocationAsync();
    Task<List<Ticket>> GetListAsync(Expression<Func<Ticket, bool>> filter);
    Task<int> SaveAsync();
}