using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class OrderTicketRepository : Repository<OrderTicket>, IOrderTicketRepository
{
    private readonly ApplicationDbContext _context;
    
    public OrderTicketRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<List<Guid>> GetTicketIdsByOrderId(Guid orderId)
    {
        return await _context.OrderTickets
            .Where(ot => ot.OrderId == orderId)
            .Select(ot => ot.TicketId)
            .ToListAsync();
    }

    public async Task<List<OrderTicket>> GetTicketsByOrderId(Guid orderId)
    {
        return await _context.OrderTickets
            .Where(ot => ot.OrderId == orderId)
            .Include(ot => ot.Ticket) 
            .ToListAsync();
    }
}