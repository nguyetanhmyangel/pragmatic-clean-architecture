using Bookify.ShareKernel.Entities;

namespace Bookify.ShareKernel.ValueObjects;

public class DateRange : ValueObject
{
    public DateOnly Start { get; }
    public DateOnly End { get; }

    public int LengthInDays => End.DayNumber - Start.DayNumber;

    private DateRange(DateOnly start, DateOnly end)
    {
        if (start > end)
            throw new InvalidOperationException("End date precedes start date");

        Start = start;
        End = end;
    }

    // For EF Core or serialization
    private DateRange()
    {
    }

    public static DateRange Create(DateOnly start, DateOnly end)
    {
        return new DateRange(start, end);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }
}