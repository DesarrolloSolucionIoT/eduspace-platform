using System.Security.Cryptography;
using System.Text;

namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;

public class RefreshToken
{
    // Parameterless ctor for EF Core
    private RefreshToken() { }

    private RefreshToken(int accountId, string tokenHash, DateTime expiresAt)
    {
        AccountId = accountId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
    }

    public int Id { get; private set; }
    public int AccountId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public int? ReplacedByTokenId { get; private set; }

    public bool IsActive => RevokedAt == null && ExpiresAt > DateTime.UtcNow;

    /// <summary>
    /// Creates a new refresh token, returning the entity (with hash stored) and the raw token for the caller.
    /// The raw token is NEVER persisted — only its SHA-256 hash is stored.
    /// </summary>
    public static (RefreshToken entity, string rawToken) CreateNew(int accountId, TimeSpan lifetime)
    {
        var rawBytes = RandomNumberGenerator.GetBytes(64);
        var rawToken = Convert.ToBase64String(rawBytes);
        var hash = ComputeHash(rawToken);
        var entity = new RefreshToken(accountId, hash, DateTime.UtcNow.Add(lifetime));
        return (entity, rawToken);
    }

    public static string ComputeHash(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
    }

    public void ReplaceWith(int newTokenId)
    {
        ReplacedByTokenId = newTokenId;
        RevokedAt = DateTime.UtcNow;
    }
}
