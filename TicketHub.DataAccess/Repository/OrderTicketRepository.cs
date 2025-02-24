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
}