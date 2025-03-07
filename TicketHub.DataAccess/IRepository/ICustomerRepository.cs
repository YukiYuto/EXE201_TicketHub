using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface ICustomerRepository : IRepository<Customer>
{
    void Update(Customer customer);
}