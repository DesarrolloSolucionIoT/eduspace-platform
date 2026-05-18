using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.IAM.Domain;

public class RefreshTokenTests
{
    // ─── CreateNew ───────────────────────────────────────────────────────────────

    [Fact]
    public void CreateNew_WhenCalled_ReturnsEntityAndNonEmptyRawToken()
    {
        // Arrange / Act
        var (entity, rawToken) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));

        // Assert
        rawToken.Should().NotBeNullOrWhiteSpace();
        entity.Should().NotBeNull();
    }

    [Fact]
    public void CreateNew_WhenCalled_StoresHashNotRawToken()
    {
        // Arrange / Act
        var (entity, rawToken) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));

        // Assert
        entity.TokenHash.Should().NotBe(rawToken);
    }

    [Fact]
    public void CreateNew_WhenCalled_HashMatchesComputeHash()
    {
        // Arrange / Act
        var (entity, rawToken) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));

        // Assert
        entity.TokenHash.Should().Be(RefreshToken.ComputeHash(rawToken));
    }

    [Fact]
    public void CreateNew_WhenCalled_SetsCorrectAccountId()
    {
        // Arrange / Act
        var (entity, _) = RefreshToken.CreateNew(accountId: 42, lifetime: TimeSpan.FromDays(14));

        // Assert
        entity.AccountId.Should().Be(42);
    }

    [Fact]
    public void CreateNew_WhenCalled_ExpiresAtIsInFuture()
    {
        // Arrange / Act
        var (entity, _) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));

        // Assert
        entity.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void CreateNew_WhenCalled_IsActiveIsTrue()
    {
        // Arrange / Act
        var (entity, _) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));

        // Assert
        entity.IsActive.Should().BeTrue();
    }

    [Fact]
    public void CreateNew_WhenCalledTwice_ProducesDifferentRawTokens()
    {
        // Arrange / Act
        var (_, rawToken1) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));
        var (_, rawToken2) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));

        // Assert
        rawToken1.Should().NotBe(rawToken2);
    }

    // ─── IsActive ────────────────────────────────────────────────────────────────

    [Fact]
    public void IsActive_WhenNotRevokedAndNotExpired_ReturnsTrue()
    {
        // Arrange
        var (entity, _) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(1));

        // Act / Assert
        entity.IsActive.Should().BeTrue();
    }

    // ─── Revoke ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Revoke_WhenCalled_SetsRevokedAt()
    {
        // Arrange
        var (entity, _) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));

        // Act
        entity.Revoke();

        // Assert
        entity.RevokedAt.Should().NotBeNull();
    }

    [Fact]
    public void Revoke_WhenCalled_IsActiveReturnsFalse()
    {
        // Arrange
        var (entity, _) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));

        // Act
        entity.Revoke();

        // Assert
        entity.IsActive.Should().BeFalse();
    }

    // ─── ReplaceWith ─────────────────────────────────────────────────────────────

    [Fact]
    public void ReplaceWith_WhenCalled_SetsReplacedByTokenId()
    {
        // Arrange
        var (entity, _) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));

        // Act
        entity.ReplaceWith(newTokenId: 99);

        // Assert
        entity.ReplacedByTokenId.Should().Be(99);
    }

    [Fact]
    public void ReplaceWith_WhenCalled_SetsRevokedAt()
    {
        // Arrange
        var (entity, _) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));

        // Act
        entity.ReplaceWith(newTokenId: 99);

        // Assert
        entity.RevokedAt.Should().NotBeNull();
    }

    [Fact]
    public void ReplaceWith_WhenCalled_IsActiveReturnsFalse()
    {
        // Arrange
        var (entity, _) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));

        // Act
        entity.ReplaceWith(newTokenId: 99);

        // Assert
        entity.IsActive.Should().BeFalse();
    }

    // ─── ComputeHash ─────────────────────────────────────────────────────────────

    [Fact]
    public void ComputeHash_WhenCalledWithSameInput_ReturnsSameHash()
    {
        // Arrange
        const string rawToken = "some_raw_token_value";

        // Act
        var hash1 = RefreshToken.ComputeHash(rawToken);
        var hash2 = RefreshToken.ComputeHash(rawToken);

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void ComputeHash_WhenCalledWithDifferentInputs_ReturnsDifferentHashes()
    {
        // Arrange / Act
        var hash1 = RefreshToken.ComputeHash("token_a");
        var hash2 = RefreshToken.ComputeHash("token_b");

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void ComputeHash_WhenCalled_ReturnsLowercaseHex()
    {
        // Arrange / Act
        var hash = RefreshToken.ComputeHash("any_token");

        // Assert
        hash.Should().MatchRegex("^[0-9a-f]+$");
    }
}
