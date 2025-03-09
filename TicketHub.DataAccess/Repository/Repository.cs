using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;

namespace TicketHub.DataAccess.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _dbSet = context.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    /// <summary>
    ///     The provided code defines a function named GetAsync that retrieves a single entity from a database table, applying
    ///     filtering and     optional property inclusion logic.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="includeProperties"></param>
    /// <returns></returns>
    public async Task<T?> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null)
    {
        IQueryable<T> query = _dbSet;
        query = query.Where(filter);

        if (!string.IsNullOrEmpty(includeProperties))
            foreach (var property in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(property);

        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    ///     The function retrieves all objects of type T from the database. It optionally includes related data specified by
    ///     the includeProperties parameter
    ///     This function first constructs a query based on the provided DbSet, and if includeProperties is not null or empty,
    ///     it iterates through the properties specified in the parameter and includes them in the query using the Include
    ///     method.
    ///     Finally, it executes the query and returns the results as a list of objects of type T.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="includeProperties"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
        string? includeProperties = null)
    {
        IQueryable<T> query = _dbSet;

        if (filter != null) query = query.Where(filter);

        if (!string.IsNullOrEmpty(includeProperties))
            foreach (var property in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(property);

        return await query.ToListAsync();
    }

    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null)
    {
        var query = _dbSet.Where(predicate);

        if (!string.IsNullOrEmpty(includeProperties))
            foreach (var includeProp in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProp);

        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> filter = null,
        string includeProperties = "")
    {
        IQueryable<T> query = _dbSet;

        if (filter != null) query = query.Where(filter);

        // Include các bảng liên quan
        foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            query = query.Include(includeProperty);

        return await query.ToListAsync();
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }
}