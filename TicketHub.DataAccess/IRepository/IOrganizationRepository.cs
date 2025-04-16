using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface IOrganizationRepository : IRepository<Organizer>
{
    void Update(Organizer organizer);

    Task<(List<Organizer> organizers, int TotalOrganizer)> GetOrganizerAsync(
        int pageNumber,
        int pageSize,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        bool isManager = false,
        string? includeProperties = null
    );
}