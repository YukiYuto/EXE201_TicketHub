using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface IOrderRepository : IRepository<Orders>
{
    void Update(Orders orders);
    void UpdateRange(IEnumerable<Orders> orders);
    Task<Orders> GetById(Guid orderId);
    
    Task<Orders> GetOrderByOrderNumber(long orderNumber);
    Task<long> GenerateUniqueNumberAsync();
    
    Task<Orders?> GetByIdAsync(Guid orderId);
}