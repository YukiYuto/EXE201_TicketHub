using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface IOrganizationRepository : IRepository<Organizer>
{
    void Update(Organizer organizer);
}