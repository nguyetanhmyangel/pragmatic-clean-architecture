namespace Bookify.ShareKernel.Specifications;

public interface ISqlSpecification<TResponse>
{
    string SqlQuery { get; }
    object Parameters { get; }
}