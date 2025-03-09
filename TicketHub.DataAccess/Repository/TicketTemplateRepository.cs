using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class TicketTemplateRepository : Repository<TicketTemplate>, ITicketTemplateRepository
{
    private readonly ApplicationDbContext _context;

    public TicketTemplateRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<TicketTemplate>, int)> GetFilteredTicketTemplatesAsync
    (
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber,
        int pageSize
    )
    {
        var query = _context.TicketTemplates.AsQueryable();

        // 🔍 **Filter**
        if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
            query = filterOn.ToLower() switch
            {
                "ticketname" => query.Where(t => t.TicketName.Contains(filterQuery)),
                "rank" => query.Where(t => t.Rank.Contains(filterQuery)),
                _ => query
            };

        query = sortBy?.ToLower() switch
        {
            "ticketname" => query.OrderBy(t => t.TicketName),
            "ticketname_desc" => query.OrderByDescending(t => t.TicketName),
            "price" => query.OrderBy(t => t.TicketPrice),
            "price_desc" => query.OrderByDescending(t => t.TicketPrice),
            _ => query.OrderBy(t => t.TicketName) // Default sort
        };

        var totalItems = await query.CountAsync();
        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return (await query.ToListAsync(), totalItems);
    }
}