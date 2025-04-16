using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<(Customer? Customer, Organizer? Organizer)> GetWithOrganizerByUserIdAsync(string userId);
    void Update(Customer customer);

    Task<(List<Customer> customers, int TotalCustomer)> GetCustomerAsync(
        int pageNumber,
        int pageSize,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        bool isManager = false,
        string? includeProperties = null
    );
}