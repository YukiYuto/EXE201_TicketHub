using Microsoft.EntityFrameworkCore;
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

    public async Task<(List<Organizer> organizers, int TotalOrganizer)> GetOrganizerAsync(int pageNumber,
        int pageSize,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        bool isManager = false,
        string? includeProperties = null)
    {
        IQueryable<Organizer> query = _context.Organizers.Include(o => o.User);

        // Include additional properties if specified
        if (!string.IsNullOrEmpty(includeProperties))
            foreach (var property in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
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
                query = query.Where(o => o.User != null && o.User.FullName.ToLower().Contains(filterQuery));
            else if (filterOn == "email")
                query = query.Where(o => o.User != null && o.User.Email.ToLower().Contains(filterQuery));
            else if (filterOn == "taxid" && isManager)
                query = query.Where(o => o.TaxId != null && o.TaxId.ToLower().Contains(filterQuery));
            else if (filterOn == "organizationname" && isManager)
                query = query.Where(o =>
                    o.OrganizationName != null && o.OrganizationName.ToLower().Contains(filterQuery));
        }

        // Sorting
        if (!string.IsNullOrEmpty(sortBy))
        {
            sortBy = sortBy.ToLower();
            query = sortBy switch
            {
                "fullname" when isManager => query.OrderBy(o => o.User != null ? o.User.FullName : string.Empty),
                "email" => query.OrderBy(o => o.User != null ? o.User.Email : string.Empty),
                "taxid" when isManager => query.OrderBy(o => o.TaxId ?? string.Empty),
                "organizationname" when isManager => query.OrderBy(o => o.OrganizationName ?? string.Empty),
                _ => query.OrderBy(o => o.OrganizerId)
            };
        }
        else
        {
            query = query.OrderBy(o => o.OrganizerId);
        }

        // Get total count before paging
        var totalCount = await query.CountAsync();

        // Paging
        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        // Execute query
        var organizers = await query.ToListAsync();

        return (organizers, totalCount);
    }
}