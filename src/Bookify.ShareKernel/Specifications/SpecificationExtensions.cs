using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bookify.ShareKernel.Specifications;

public static class SpecificationExtensions
{
    public static IQueryable<TEntity> WithSpecification<TEntity>(this DbSet<TEntity> dbSet, ISpecification<TEntity> spec)
        where TEntity : class
    {
        return SpecificationEvaluator.Default.GetQuery(dbSet.AsNoTracking(), spec);
    }

    public static IQueryable<TResult> WithSpecification<TEntity, TResult>(this DbSet<TEntity> dbSet, IProjectionSpecification<TEntity, TResult> spec)
        where TEntity : class
    {
        var query = SpecificationEvaluator.Default.GetQuery(dbSet.AsNoTracking(), spec);
        return query.Select(spec.Selector!); // ! vì Selector có thể nullable
    }

    public static TResult WithAggregateSpecification<TEntity, TResult>(this DbSet<TEntity> dbSet, IAggregateSpecification<TEntity, TResult> spec)
        where TEntity : class
    {
        var query = SpecificationEvaluator.Default.GetQuery(dbSet.AsQueryable(), spec);
        return spec.AggregateSelector.Compile()(query); // Compile để chạy Expression
    }
}

