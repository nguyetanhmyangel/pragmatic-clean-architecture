using Microsoft.EntityFrameworkCore;
using Bookify.ShareKernel.Specification;

namespace Bookify.Application.Abstractions.Extensions;

public static class SpecificationExtensions
{
    public static IQueryable<T> ApplySpecification<T>(this IQueryable<T> query, ISpecification<T> spec)
        where T : class
    {
        if (spec.Criteria is not null)
            query = query.Where(spec.Criteria);

        foreach (var include in spec.Includes)
            query = query.Include(include);

        if (spec.OrderBy is not null)
            query = spec.OrderBy(query);
        else if (spec.OrderByDescending is not null)
            query = spec.OrderByDescending(query);

        return query;
    }
}