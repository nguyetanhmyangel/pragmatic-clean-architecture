using System.Linq.Expressions;
using Ardalis.Specification;

namespace Bookify.ShareKernel.Specifications;

// Dùng cho việc chiếu (selecting specific columns / DTO)
public interface IProjectionSpecification<TEntity, TResult> : ISpecification<TEntity>
{
    Expression<Func<TEntity, TResult>> Selector { get; }
}
