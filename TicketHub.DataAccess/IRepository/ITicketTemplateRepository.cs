using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface ITicketTemplateRepository : IRepository<TicketTemplate>
{
    Task<(IEnumerable<TicketTemplate>, int)> GetFilteredTicketTemplatesAsync(
        string? filterOn, string? filterQuery, string? sortBy, int pageNumber, int pageSize);

    void Update(TicketTemplate ticketTemplate);
}