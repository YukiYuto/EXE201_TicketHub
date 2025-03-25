using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Category category)
    {
        _context.Categories.Update(category);
    }

    public void UpdateRange(IEnumerable<Category> categories)
    {
        _context.Categories.UpdateRange(categories);
    }

    public async Task<Category> GetById(Guid categoryId)
    {
        return await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == categoryId);
    }

    public async Task<List<Category>> GetSubcategories(Guid categoryId)
    {
        return await _context.Categories
            .Where(c => c.ParentCategoryId == categoryId)
            .ToListAsync();
    }
}