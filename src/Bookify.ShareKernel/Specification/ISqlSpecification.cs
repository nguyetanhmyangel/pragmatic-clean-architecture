namespace Bookify.ShareKernel.Specification;

public interface ISqlSpecification<TResponse>
{
    string SqlQuery { get; }
    object Parameters { get; }
}