using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class TicketSerialNumberRepository : Repository<TicketSerialNumber>, ITicketSerialNumberRepository
{
    private readonly ApplicationDbContext _context;

    public TicketSerialNumberRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(TicketSerialNumber ticketSerialNumber)
    {
        _context.TicketSerialNumbers.Update(ticketSerialNumber);
    }

    public async Task<(IEnumerable<TicketSerialNumber>, int)> GetTicketSerialNumberAsync
    (
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber,
        int pageSize,
        Guid? ticketTemplateId = null
    )
    {
        IQueryable<TicketSerialNumber> query = _context.TicketSerialNumbers.Include(t => t.TicketTemplate);

        // Áp dụng bộ lọc nếu có
        if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
            switch (filterOn.ToLower())
            {
                case "serialnumber":
                    query = query.Where(t => t.SerialNumber.Contains(filterQuery));
                    break;
                case "tickettemplateid":
                    query = query.Where(t => t.TicketTemplateId.ToString().Contains(filterQuery));
                    break;
            }

        // Áp dụng sắp xếp
        if (!string.IsNullOrEmpty(sortBy))
        {
            var sortParams = sortBy.Trim().ToLower().Split('_');
            var sortField = sortParams[0];
            var sortDirection = sortParams.Length > 1 ? sortParams[1] : "asc";

            query = sortField switch
            {
                "serialnumber" => sortDirection == "desc"
                    ? query.OrderByDescending(t => t.SerialNumber)
                    : query.OrderBy(t => t.SerialNumber),
                _ => query.OrderBy(t => t.SerialNumber) // Sắp xếp mặc định
            };
        }

        // Đếm tổng số bản ghi
        var totalItems = await query.CountAsync();

        // Áp dụng phân trang
        var ticketSerialNumbers = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (ticketSerialNumbers, totalItems);
    }
}