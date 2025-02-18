using Microsoft.AspNetCore.Identity;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public ITicketRepository TicketRepository { get; set; }
    public IEventRepository EventRepository { get; set; }
    public ICartRepository CartRepository { get; set; }
    public ICartItemRepository CartItemRepository { get; set; }

    public ICategoryRepository CategoryRepository { get; set; }
    public IOrderRepository OrderRepository { get; set; }
    
    public UnitOfWork(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        TicketRepository = new TicketRepository(_context);
        EventRepository = new EventRepository(_context);
        CartRepository = new CartRepository(_context);
        CartItemRepository = new CartItemRepository(_context);
        CategoryRepository = new CategoryRepository(_context);
        OrderRepository = new OrderRepository(_context);
    }
    
    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }
}