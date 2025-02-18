using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class CartRepository : Repository<Cart>, ICartRepository
{
    private readonly ApplicationDbContext _context;

    public CartRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Cart carts)
    {
        _context.Carts.Update(carts);
    }

    public void UpdateRange(IEnumerable<Cart> carts)
    {
        _context.Carts.UpdateRange(carts);
    }

    public async Task<Cart> GetById(Guid cartId)
    {
        return await _context.Carts.FirstOrDefaultAsync(x => x.CartId == cartId);
    }
}