namespace Bookify.ShareKernel.Time;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}