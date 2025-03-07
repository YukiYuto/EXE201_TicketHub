namespace TicketHub.DataAccess.IRepository;

public interface IUnitOfWork
{
    ICustomerRepository CustomerRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    ITicketRepository TicketRepository { get; }
    ITicketTemplateRepository TicketTemplateRepository { get; }
    IEventRepository EventRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    ICartRepository CartRepository { get; }
    ICartItemRepository CartItemRepository { get; }

    IOrderRepository OrderRepository { get; }

    //IOrderTicketRepository OrderTicketRepository { get; }
    IPaymentRepository PaymentRepository { get; }
    ITransactionRepository TransactionRepository { get; }
    Task<int> SaveAsync();
}