using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface ITicketSerialNumberRepository : IRepository<TicketSerialNumber>
{
    void Update(TicketSerialNumber ticketSerialNumber);

    Task<(IEnumerable<TicketSerialNumber>, int)> GetTicketSerialNumberAsync
    (
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber,
        int pageSize,
        Guid? ticketTemplateId = null
    );
}