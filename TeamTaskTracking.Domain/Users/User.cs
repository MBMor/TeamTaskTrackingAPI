using System;
using System.Collections.Generic;
using System.Text;

namespace TeamTaskTracking.Domain.Users;

public sealed class User
{
    public Guid Id { get; set; }
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }

    private User()
    { }
    public User(string email, string firstName, string lastName)
    {
        Id = Guid.NewGuid();
        SetEmail(email);
        SetFirstName(firstName);
        SetLastName(lastName);
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrEmpty(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));

        PasswordHash = passwordHash;
    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new ArgumentException("Email is required.", nameof(email));

        Email = email.Trim().ToLowerInvariant();
    }

    private void SetFirstName(string firstName)
    {
        if (string.IsNullOrEmpty(firstName))
            throw new ArgumentException("First name is required.", nameof(firstName));

        if (firstName.Length > 100)
            throw new ArgumentException("First name must be 100 characters or fewer.", nameof(firstName));


        FirstName = firstName.Trim();
    }

    private void SetLastName(string lastName)
    {
        if (string.IsNullOrEmpty(lastName))
            throw new ArgumentException("Last name is required.", nameof(lastName));

        if (lastName.Length > 100)
            throw new ArgumentException("Last name must be 100 characters or fewer.", nameof(lastName));


        LastName = lastName.Trim();
    }

}
