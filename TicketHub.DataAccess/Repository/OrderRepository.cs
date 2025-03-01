using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class OrderRepository : Repository<Orders>, IOrderRepository
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

    /*public async Task<Orders> GetById(Guid orderId)
    {
        return await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);
    }*/

    public async Task<Orders> GetOrderByOrderNumber(long orderNumber)
    {
        return await _context.Orders.FirstOrDefaultAsync(x => x.OrderNumber == orderNumber);
    }

    public async Task<long> GenerateUniqueNumberAsync()
    {
        // Tìm số lớn nhất hiện có trong bảng Orders
        var maxOrderNumber = await _context.Orders.MaxAsync(o => (long?)o.OrderNumber) ?? 0;

        // Tạo số mới lớn hơn số lớn nhất 1 đơn vị
        return maxOrderNumber + 1;
    }
    
    public async Task<Orders?> GetByIdAsync(Guid orderId)
    {
        return await _context.Orders.FindAsync(orderId);
    }
}