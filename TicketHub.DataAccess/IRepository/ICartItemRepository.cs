using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface ICartItemRepository : IRepository<CartItem>
{
    void Update(CartItem cartItem);
    void UpdateRange(IEnumerable<CartItem> cartItems);
}