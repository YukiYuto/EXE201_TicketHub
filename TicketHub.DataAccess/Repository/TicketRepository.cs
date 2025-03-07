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

    public async Task<Ticket> GeTicketById(Guid ticketId)
    {
        return await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId);
    }

    public void Update(Ticket ticket)
    {
        _context.Tickets.Update(ticket);
    }

    public void UpdateRange(IEnumerable<Ticket> tickets)
    {
        _context.Tickets.UpdateRange(tickets);
    }

    /*public async Task<IEnumerable<Ticket>> GetAllWithEventAndLocationAsync()
    {
        return await _context.Tickets
            .Include(t => t.Event)
            .Include(ticket => ticket.Category)
            .ToListAsync();
    }*/

    public async Task<List<Ticket>> GetListAsync(Expression<Func<Ticket, bool>> filter)
    {
        return await _context.Tickets.Where(filter).ToListAsync();
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }
}