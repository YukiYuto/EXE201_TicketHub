namespace TicketHub.DataAccess.IRepository;

public interface IUnitOfWork
{
    ITicketRepository TicketRepository { get; }
    IEventRepository EventRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    ICartRepository CartRepository { get; }
    ICartItemRepository CartItemRepository { get; }
    IOrderRepository OrderRepository { get; }
    Task<int> SaveAsync();
}