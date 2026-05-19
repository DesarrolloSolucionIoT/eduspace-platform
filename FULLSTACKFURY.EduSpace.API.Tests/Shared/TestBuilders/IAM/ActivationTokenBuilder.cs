using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;

namespace FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.IAM;

/// <summary>
/// Fluent builder for <see cref="ActivationToken"/> test instances.
/// Uses CreateNew for a valid baseline and allows overriding ExpiresAt via reflection
/// to seed past-expiry scenarios without introducing a time abstraction.
/// </summary>
public class ActivationTokenBuilder
{
    private int _accountId = 1;
    private TimeSpan _lifetime = TimeSpan.FromHours(24);
    private DateTime? _expiresAtOverride;

    public ActivationTokenBuilder WithAccountId(int accountId)
    {
        _accountId = accountId;
        return this;
    }

    public ActivationTokenBuilder WithLifetime(TimeSpan lifetime)
    {
        _lifetime = lifetime;
        return this;
    }

    /// <summary>
    /// Overrides ExpiresAt after creation to simulate expired tokens.
    /// </summary>
    public ActivationTokenBuilder WithExpiresAt(DateTime expiresAt)
    {
        _expiresAtOverride = expiresAt;
        return this;
    }

    public ActivationToken Build()
    {
        var (entity, _) = ActivationToken.CreateNew(_accountId, _lifetime);

        if (_expiresAtOverride.HasValue)
        {
            var prop = typeof(ActivationToken).GetProperty(nameof(ActivationToken.ExpiresAt));
            prop!.SetValue(entity, _expiresAtOverride.Value);
        }

        return entity;
    }
}
