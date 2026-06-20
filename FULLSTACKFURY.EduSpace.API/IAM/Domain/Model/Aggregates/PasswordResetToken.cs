using System.Security.Cryptography;
using System.Text;

namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;

public class PasswordResetToken
{
    // Parameterless ctor for EF Core
    private PasswordResetToken() { }

    private PasswordResetToken(int accountId, string tokenHash, DateTime expiresAt)
    {
        AccountId = accountId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
    }

    public int Id { get; private set; }
    public int AccountId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }

    /// <summary>
    /// Returns true when the token has not been used and has not expired.
    /// </summary>
    public bool IsValid() => UsedAt == null && ExpiresAt > DateTime.UtcNow;

    /// <summary>
    /// Creates a new password-reset token returning the entity (hash stored) and the raw token for the caller.
    /// The raw token is NEVER persisted — only its SHA-256 hash is stored.
    /// Raw token = 32 bytes from CSPRNG, base64-url encoded (no padding).
    /// </summary>
    public static (PasswordResetToken entity, string rawToken) CreateNew(int accountId, TimeSpan lifetime)
    {
        var rawBytes = RandomNumberGenerator.GetBytes(32);
        var rawToken = Convert.ToBase64String(rawBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
        var hash = ComputeHash(rawToken);
        var entity = new PasswordResetToken(accountId, hash, DateTime.UtcNow.Add(lifetime));
        return (entity, rawToken);
    }

    public static string ComputeHash(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    /// <summary>
    /// Marks the token as used. Throws if already used (single-use invariant).
    /// </summary>
    public void MarkAsUsed()
    {
        if (UsedAt != null)
            throw new InvalidOperationException("This password-reset token has already been used.");

        UsedAt = DateTime.UtcNow;
    }
}
