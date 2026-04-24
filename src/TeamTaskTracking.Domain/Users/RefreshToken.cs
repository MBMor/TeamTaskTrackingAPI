namespace TeamTaskTracking.Domain.Users;

public sealed class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid TokenFamilyId { get; private set; }   
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }
    public bool IsCompromised { get; private set; }

    private RefreshToken()
    {
    }

    public RefreshToken(
        Guid userId,
        Guid tokenFamilyId,
        string tokenHash,
        DateTime expiresAtUtc)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));

        if (tokenFamilyId == Guid.Empty)
            throw new ArgumentException("TokenFamilyId is required.", nameof(tokenFamilyId));

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException("Token hash is required.", nameof(tokenHash));

        if (expiresAtUtc <= DateTime.UtcNow)
            throw new ArgumentException("Refresh token expiration must be in the future.", nameof(expiresAtUtc));

        Id = Guid.NewGuid();
        UserId = userId;
        TokenFamilyId = tokenFamilyId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public bool IsActive => RevokedAtUtc is null && ExpiresAtUtc > DateTime.UtcNow && !IsCompromised;

    public void Revoke(string? replacedByTokenHash = null)
    {
        if (RevokedAtUtc is not null)
            return;

        RevokedAtUtc = DateTime.UtcNow;
        ReplacedByTokenHash = string.IsNullOrWhiteSpace(replacedByTokenHash)
            ? null
            : replacedByTokenHash;
    }

    public void MarkCompromised()
    {
        IsCompromised = true;

        if (RevokedAtUtc is null)
        {
            RevokedAtUtc = DateTime.UtcNow;
        }
    }
}