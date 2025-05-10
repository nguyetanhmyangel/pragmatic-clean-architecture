using Bookify.ShareKernel.Utilities;

namespace Bookify.Infrastructure.Utilities;
internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
