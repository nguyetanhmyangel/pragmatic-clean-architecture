using Ardalis.Specification;
using Bookify.ShareKernel.Entities;
using Bookify.ShareKernel.Specifications;

namespace Bookify.ShareKernel.Repositories;

public interface IRepository<TEntity, TEntityId>
    where TEntity : Entity<TEntityId>
    where TEntityId : class
{
    Task<TEntity?> GetByIdAsync(TEntityId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, 
        CancellationToken cancellationToken = default);
    Task<TEntity?> GetSingleAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    // ✨ Thêm projection support vào interface
    Task<IReadOnlyList<TResult>> ListAsync<TResult>(IProjectionSpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default);
    Task<TEntity?> GetSingleAsync<TResult>(IProjectionSpecification<TEntity, TResult> specification,
        CancellationToken cancellationToken = default);
    TEntity? ExecuteAggregate<TResult>(IAggregateSpecification<TEntity, TResult> specification);
    Task<TEntity?> ExecuteAggregateAsync<TResult>(IAggregateSpecification<TEntity, TResult> specification,
        CancellationToken cancellationToken = default);

    // Mutating methods
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
}





