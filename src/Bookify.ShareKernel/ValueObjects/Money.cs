using Bookify.ShareKernel.Entities;

namespace Bookify.ShareKernel.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    public Money(decimal amount, Currency currency)
    {
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        Amount = amount;
    }

    public static Money operator +(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            throw new InvalidOperationException("Currencies have to be equal");
        }

        return new Money(first.Amount + second.Amount, first.Currency);
    }

    public static Money Zero() => new Money(0, Currency.None);

    public static Money Zero(Currency currency) => new Money(0, currency);

    public bool IsZero() => this == Zero(Currency);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
