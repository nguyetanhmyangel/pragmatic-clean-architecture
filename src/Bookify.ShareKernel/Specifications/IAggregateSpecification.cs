using System.Linq.Expressions;
using Ardalis.Specification;

namespace Bookify.ShareKernel.Specifications;

// // Dùng cho các hàm Aggregate (SUM, MIN, MAX, COUNT)
public interface IAggregateSpecification<TEntity, TResult> : ISpecification<TEntity>
{
    Expression<Func<IQueryable<TEntity>, TResult>> AggregateSelector { get; }
}
