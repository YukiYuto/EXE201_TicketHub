using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class EventRepository : Repository<Event>, IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Event events)
    {
        _context.Events.Update(events);
    }

    public void UpdateRange(IEnumerable<Event> events)
    {
        _context.Events.UpdateRange(events);
    }

    public async Task<Event> GetById(Guid eventId)
    {
        return await _context.Events.FirstOrDefaultAsync(x => x.EventId == eventId);
    }
}