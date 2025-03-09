using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    private readonly ApplicationDbContext _context;

    public OrderDetailRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    /*public async Task<List<Guid>> GetTicketIdsByOrderId(Guid orderId)
    {
        /*return await _context.OrderDetails
            .Where(x => x.OrderId == orderId)
            .Select(x => x.TicketId)
            .ToListAsync();#1#
        return null;
    }*/

    public async Task<List<OrderDetail>> GetListByOrderIdAsync(Guid orderId)
    {
        return await _context.Set<OrderDetail>()
            .Where(od => od.OrderId == orderId)
            .ToListAsync();
    }
}