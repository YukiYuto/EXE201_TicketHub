using System.Linq.Expressions;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface ITicketRepository : IRepository<Ticket>
{
    Task<IEnumerable<Ticket>> GetAllAsync(
        Expression<Func<Ticket, bool>> filter,
        Func<IQueryable<Ticket>, IQueryable<Ticket>> includes
    );

    Task<Ticket> GeTicketById(Guid ticketId);
}