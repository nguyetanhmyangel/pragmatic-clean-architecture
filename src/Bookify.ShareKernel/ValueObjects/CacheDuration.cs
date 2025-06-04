using Bookify.ShareKernel.Entities;

namespace Bookify.ShareKernel.ValueObjects;

public sealed class CacheDuration : ValueObject
{
    public static readonly CacheDuration Short = new(TimeSpan.FromMinutes(5));
    public static readonly CacheDuration Medium = new(TimeSpan.FromMinutes(30));
    public static readonly CacheDuration Long = new(TimeSpan.FromHours(1));

    private TimeSpan Duration { get; }

    private CacheDuration(TimeSpan duration)
    {
        Duration = duration;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Duration;
    }

    public override string ToString() => Duration.ToString();
}
