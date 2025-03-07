using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class OrganizationRepository : Repository<Organizer>, IOrganizationRepository
{
    private readonly ApplicationDbContext _context;

    public OrganizationRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Organizer organizer)
    {
        _context.Organizers.Update(organizer);
    }
}