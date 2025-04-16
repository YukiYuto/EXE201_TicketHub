using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<(Customer? Customer, Organizer? Organizer)> GetWithOrganizerByUserIdAsync(string userId)
    {
        var customer = await _context.Set<Customer>()
            .FirstOrDefaultAsync(c => c.UserId == userId);
        var organizer = await _context.Set<Organizer>()
            .FirstOrDefaultAsync(o => o.UserId == userId);

        return (customer, organizer);
    }

    public void Update(Customer customer)
    {
        _context.Customer.Update(customer);
    }

    public async Task<(List<Customer> customers, int TotalCustomer)> GetCustomerAsync(
        int pageNumber,
        int pageSize,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        bool isManager = false,
        string? includeProperties = null)
    {
        // Start with the base query and include the User navigation property by default
        IQueryable<Customer> query = _context.Customer.Include(c => c.User);

        // Include additional properties if specified, excluding 'User' since it's already included
        if (!string.IsNullOrEmpty(includeProperties))
            foreach (var property in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                // Skip 'User' or 'ApplicationUser' since User is already included
                if (property.Equals("User", StringComparison.OrdinalIgnoreCase) ||
                    property.Equals("ApplicationUser", StringComparison.OrdinalIgnoreCase))
                    continue;
                query = query.Include(property);
            }

        // Filtering
        if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
        {
            filterOn = filterOn.ToLower();
            filterQuery = filterQuery.ToLower();

            if (filterOn == "fullname" && isManager)
                query = query.Where(c => c.User != null && c.User.FullName.ToLower().Contains(filterQuery));
            else if (filterOn == "email")
                query = query.Where(c => c.User != null && c.User.Email.ToLower().Contains(filterQuery));
            else if (filterOn == "cccd" && isManager)
                query = query.Where(c => c.CCCD != null && c.CCCD.ToLower().Contains(filterQuery));
        }

        // Sorting
        if (!string.IsNullOrEmpty(sortBy))
        {
            sortBy = sortBy.ToLower();
            query = sortBy switch
            {
                "fullname" when isManager => query.OrderBy(c => c.User != null ? c.User.FullName : string.Empty),
                "email" => query.OrderBy(c => c.User != null ? c.User.Email : string.Empty),
                "cccd" when isManager => query.OrderBy(c => c.CCCD ?? string.Empty),
                _ => query.OrderBy(c => c.CustomerId)
            };
        }
        else
        {
            query = query.OrderBy(c => c.CustomerId);
        }

        // Get total count before paging
        var totalCount = await query.CountAsync();

        // Paging
        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        // Execute query
        var customers = await query.ToListAsync();

        return (customers, totalCount);
    }
}