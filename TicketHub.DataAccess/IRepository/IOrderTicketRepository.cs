using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface IOrderTicketRepository : IRepository<OrderTicket>
{
    Task<List<Guid>> GetTicketIdsByOrderId(Guid orderId);
    Task<List<OrderTicket>> GetTicketsByOrderId(Guid orderId);
}