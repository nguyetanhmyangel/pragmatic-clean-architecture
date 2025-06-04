using System.Text.RegularExpressions;
using Bookify.ShareKernel.Entities;
using Bookify.ShareKernel.Errors;
using Bookify.ShareKernel.Utilities;

namespace Bookify.ShareKernel.ValueObjects;
public sealed class Email : ValueObject
{
    public string Value { get; }

    // EF Core sẽ dùng constructor này khi materialize object từ DB
    private Email(string value) => Value = value;

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Services cannot be empty", nameof(email));

        if (email.Length > 254)
            throw new ArgumentException("Services is too long", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        return new Email(email);
    }

    private static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email,  @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value.ToLower();
    }

    public override string ToString() => Value;
}



