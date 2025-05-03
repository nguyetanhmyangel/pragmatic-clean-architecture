using System.Linq.Expressions;
using Bookify.ShareKernel.BaseEntity;

namespace Bookify.ShareKernel.BaseRepository;

public interface IRepository<TEntity, TEntityId> where TEntity : Entity<TEntityId>
{
    Task<TEntity?> GetByIdAsync(TEntityId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    IQueryable<TEntity> AsQueryable();
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
}
