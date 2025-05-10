namespace Bookify.ShareKernel.Utilities;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}