using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface IEventRepository : IRepository<Event>
{
    void Update(Event events);
    void UpdateRange(IEnumerable<Event> events);
    Task<Event> GetById(Guid eventId);
}