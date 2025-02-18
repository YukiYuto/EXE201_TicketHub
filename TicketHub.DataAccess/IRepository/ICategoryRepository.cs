using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface ICategoryRepository : IRepository<Category>
{
    void Update(Category category);
    void UpdateRange(IEnumerable<Category> categories);
    Task<Category> GetById(Guid categoryId);
    Task<List<Category>> GetSubcategories(Guid categoryId);
}