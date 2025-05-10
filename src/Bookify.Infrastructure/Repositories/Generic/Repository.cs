using System.Linq.Expressions;
using Bookify.ShareKernel.Entities;
using Bookify.ShareKernel.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories.Generic;
public class Repository<TEntity, TEntityId>(DbContext dbContext) : IRepository<TEntity, TEntityId>
    where TEntity : Entity<TEntityId>
    where TEntityId : class
{
    protected readonly DbContext DbContext = dbContext;

    public async Task<TEntity?> GetByIdAsync(TEntityId id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>().FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>().ToListAsync(cancellationToken);
    }
    
    public IQueryable<TEntity> AsQueryable()
    {
        return DbContext.Set<TEntity>().AsQueryable();
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>().AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        return predicate == null
            ? await DbContext.Set<TEntity>().CountAsync(cancellationToken)
            : await DbContext.Set<TEntity>().CountAsync(predicate, cancellationToken);
    }

    public virtual void Add(TEntity entity)
    {
        DbContext.Set<TEntity>().Add(entity);
    }

    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        DbContext.Set<TEntity>().AddRange(entities);
    }

    public virtual void Update(TEntity entity)
    {
        DbContext.Set<TEntity>().Update(entity);
    }

    public virtual void Remove(TEntity entity)
    {
        DbContext.Set<TEntity>().Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        DbContext.Set<TEntity>().RemoveRange(entities);
    }
}

