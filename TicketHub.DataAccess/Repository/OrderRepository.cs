using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class OrderRepository :  Repository<Orders>, IOrderRepository
{
    private readonly ApplicationDbContext _context;


    public OrderRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Orders orders)
    {
        _context.Orders.Update(orders);
    }

    public void UpdateRange(IEnumerable<Orders> orders)
    {
        _context.Orders.UpdateRange(orders);
    }

    public async Task<Orders> GetById(Guid orderId)
    {
        return await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);
    }

    public async Task<Orders> GetAppointmentByOrderNumber(long orderNumber)
    {
        return await _context.Orders.FirstOrDefaultAsync(x => x.OrderNumber == orderNumber);
    }
}