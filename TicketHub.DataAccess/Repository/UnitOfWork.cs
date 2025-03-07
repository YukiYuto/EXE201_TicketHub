using Microsoft.AspNetCore.Identity;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;


    public UnitOfWork(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        TicketRepository = new TicketRepository(_context);
        TicketTemplateRepository = new TicketTemplateRepository(_context);
        EventRepository = new EventRepository(_context);
        CartRepository = new CartRepository(_context);
        CartItemRepository = new CartItemRepository(_context);
        CategoryRepository = new CategoryRepository(_context);
        OrderRepository = new OrderRepository(_context);
        OrderDetailRepository = new OrderDetailRepository(_context);
        PaymentRepository = new PaymentRepository(_context);
        TransactionRepository = new TransactionRepository(_context);
        CustomerRepository = new CustomerRepository(_context);
        OrganizationRepository = new OrganizationRepository(_context);
    }


    public ICustomerRepository CustomerRepository { get; }
    public IOrganizationRepository OrganizationRepository { get; }
    public ITicketRepository TicketRepository { get; set; }
    public ITicketTemplateRepository TicketTemplateRepository { get; set; }
    public IEventRepository EventRepository { get; set; }
    public ICartRepository CartRepository { get; set; }
    public ICartItemRepository CartItemRepository { get; set; }

    public ICategoryRepository CategoryRepository { get; set; }

    public IOrderRepository OrderRepository { get; set; }
    public IOrderDetailRepository OrderDetailRepository { get; set; }

    public IPaymentRepository PaymentRepository { get; set; }
    public ITransactionRepository TransactionRepository { get; set; }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }
}