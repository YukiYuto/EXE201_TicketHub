using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface ICartRepository : IRepository<Cart>
{
    void Update(Cart cart);
    void UpdateRange(IEnumerable<Cart> carts);
    Task<Cart> GetById(Guid cart);
}