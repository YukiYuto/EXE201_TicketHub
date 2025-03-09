using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface IOrderDetailRepository : IRepository<OrderDetail>
{
    //Task<List<Guid>> GetTicketIdsByOrderId(Guid orderId);
    Task<List<OrderDetail>> GetListByOrderIdAsync(Guid orderId);
}