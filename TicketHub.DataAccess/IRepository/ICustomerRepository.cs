using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<(Customer? Customer, Organizer? Organizer)> GetWithOrganizerByUserIdAsync(string userId);
    void Update(Customer customer);
}