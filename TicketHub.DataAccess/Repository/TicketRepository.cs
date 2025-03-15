using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class TicketRepository : Repository<Ticket>, ITicketRepository
{
    private readonly ApplicationDbContext _context;

    public TicketRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Ticket>> GetAllAsync(Expression<Func<Ticket, bool>> filter,
        Func<IQueryable<Ticket>, IQueryable<Ticket>> includes)
    {
        IQueryable<Ticket> query = _context.Tickets;

        if (filter != null) query = query.Where(filter);

        if (includes != null) query = includes(query);

        return await query.AsNoTracking().ToListAsync();
    }
}