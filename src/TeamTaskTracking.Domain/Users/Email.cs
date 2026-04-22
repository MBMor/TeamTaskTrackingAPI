using System.Text.RegularExpressions;

namespace TeamTaskTracking.Domain.Users;

public sealed class Email : IEquatable<Email>
{
    public string Value { get; }

    private Email(string value)
    {  Value = value; }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email is required.", nameof(value));

        var normalized = Normalize(value);

        if (normalized.Length > 254)
            throw new ArgumentException("Email must not exceed 254 characters.", nameof(value));

        if (!IsValidFormat(normalized))
            throw new ArgumentException("Email format is invalid.", nameof(value));

        return new Email(normalized);
    }

    private static string Normalize(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    private static bool IsValidFormat(string value)
    {
        return Regex.IsMatch(
            value,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.CultureInvariant);
    }
    public bool Equals(Email? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Email other && Equals(other); 
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode(StringComparison.Ordinal);
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(Email email)
    {
        return email.Value;
    }
}
