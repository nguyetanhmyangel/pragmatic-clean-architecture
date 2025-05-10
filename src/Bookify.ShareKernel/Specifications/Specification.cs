using System.Linq.Expressions;

namespace Bookify.ShareKernel.Specifications;

public abstract class Specification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; protected set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; protected set; }
    public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderByDescending { get; protected set; }

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected void ApplyOrderBy(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
    {
        OrderBy = orderBy;
    }

    protected void ApplyOrderByDescending(Func<IQueryable<T>, IOrderedQueryable<T>> orderByDescending)
    {
        OrderByDescending = orderByDescending;
    }
}