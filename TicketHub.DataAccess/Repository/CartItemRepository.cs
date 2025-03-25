using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class CartItemRepository : Repository<CartItem>, ICartItemRepository
{
    private readonly ApplicationDbContext _context;

    public CartItemRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(CartItem cartItem)
    {
        _context.CartItems.Update(cartItem);
    }

    public void UpdateRange(IEnumerable<CartItem> cartItems)
    {
        _context.CartItems.UpdateRange(cartItems);
    }
}