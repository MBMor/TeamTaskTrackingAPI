namespace TeamTaskTracking.Domain.Users;

public sealed class User
{
    public Guid Id { get; set; }
    public Email Email { get; private set; } = null!;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }  
    public DateTime CreatedAtUtc { get; set; }

    private User()
    { }
    public User(Email email, string firstName, string lastName)
    {
        Id = Guid.NewGuid();
        SetEmail(email);
        SetFirstName(firstName);
        SetLastName(lastName);
        Role = UserRole.User;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrEmpty(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));

        PasswordHash = passwordHash;
    }

    private void SetEmail(Email email)
    {
        Email = email;
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
