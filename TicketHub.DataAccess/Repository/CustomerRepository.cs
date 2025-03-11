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
}