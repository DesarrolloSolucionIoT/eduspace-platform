using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.IAM;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.IAM.Domain;

public class ActivationTokenTests
{
    // ─── CreateNew ───────────────────────────────────────────────────────────────

    [Fact]
    public void CreateNew_WhenCalled_ReturnsEntityAndNonEmptyRawToken()
    {
        // Arrange / Act
        var (entity, rawToken) = ActivationToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromHours(24));

        // Assert
        rawToken.Should().NotBeNullOrWhiteSpace();
        entity.Should().NotBeNull();
    }

    [Fact]
    public void CreateNew_WhenCalled_RawTokenIsDistinctFromHash()
    {
        // Arrange / Act
        var (entity, rawToken) = ActivationToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromHours(24));

        // Assert
        entity.TokenHash.Should().NotBe(rawToken);
    }

    [Fact]
    public void CreateNew_WhenCalled_HashMatchesSha256OfRawToken()
    {
        // Arrange / Act
        var (entity, rawToken) = ActivationToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromHours(24));

        // Assert
        entity.TokenHash.Should().Be(ActivationToken.ComputeHash(rawToken));
    }

    [Fact]
    public void CreateNew_WhenCalled_RawTokenIsBase64UrlAtLeast43Chars()
    {
        // Arrange / Act
        var (_, rawToken) = ActivationToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromHours(24));

        // Assert — 32 raw bytes → 43 base64url chars (no padding)
        rawToken.Length.Should().BeGreaterThanOrEqualTo(43);
    }

    [Fact]
    public void CreateNew_WhenCalled_RawTokenContainsOnlyBase64UrlChars()
    {
        // Arrange / Act
        var (_, rawToken) = ActivationToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromHours(24));

        // Assert — base64url uses A-Z, a-z, 0-9, -, _ (no + / or padding =)
        rawToken.Should().MatchRegex("^[A-Za-z0-9_-]+$");
    }

    [Fact]
    public void CreateNew_WhenCalled_SetsCorrectAccountId()
    {
        // Arrange / Act
        var (entity, _) = ActivationToken.CreateNew(accountId: 42, lifetime: TimeSpan.FromHours(24));

        // Assert
        entity.AccountId.Should().Be(42);
    }

    [Fact]
    public void CreateNew_WhenCalled_ExpiresAtIsInFuture()
    {
        // Arrange / Act
        var (entity, _) = ActivationToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromHours(24));

        // Assert
        entity.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void CreateNew_WhenCalled_UsedAtIsNull()
    {
        // Arrange / Act
        var (entity, _) = ActivationToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromHours(24));

        // Assert
        entity.UsedAt.Should().BeNull();
    }

    [Fact]
    public void CreateNew_WhenCalledTwice_ProducesDifferentRawTokens()
    {
        // Arrange / Act
        var (_, rawToken1) = ActivationToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromHours(24));
        var (_, rawToken2) = ActivationToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromHours(24));

        // Assert
        rawToken1.Should().NotBe(rawToken2);
    }

    [Fact]
    public void CreateNew_WhenCalled_TokenHashIs64LowercaseHexChars()
    {
        // Arrange / Act
        var (entity, _) = ActivationToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromHours(24));

        // Assert — SHA-256 produces 32 bytes → 64 hex chars
        entity.TokenHash.Length.Should().Be(64);
        entity.TokenHash.Should().MatchRegex("^[0-9a-f]+$");
    }

    // ─── MarkAsUsed ───────────────────────────────────────────────────────────────

    [Fact]
    public void MarkAsUsed_WhenNotUsed_SetsUsedAt()
    {
        // Arrange
        var (entity, _) = ActivationToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromHours(24));

        // Act
        entity.MarkAsUsed();

        // Assert
        entity.UsedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsUsed_WhenNotUsed_UsedAtIsApproximatelyNow()
    {
        // Arrange
        var before = DateTime.UtcNow;
        var (entity, _) = ActivationToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromHours(24));

        // Act
        entity.MarkAsUsed();

        // Assert
        entity.UsedAt.Should().BeOnOrAfter(before);
        entity.UsedAt.Should().BeOnOrBefore(DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void MarkAsUsed_WhenAlreadyUsed_Throws()
    {
        // Arrange
        var (entity, _) = ActivationToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromHours(24));
        entity.MarkAsUsed();

        // Act
        Action act = () => entity.MarkAsUsed();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    // ─── IsValid ─────────────────────────────────────────────────────────────────

    [Fact]
    public void IsValid_WhenNotUsedAndNotExpired_ReturnsTrue()
    {
        // Arrange
        var (entity, _) = ActivationToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromHours(24));

        // Assert
        entity.IsValid().Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenUsed_ReturnsFalse()
    {
        // Arrange
        var (entity, _) = ActivationToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromHours(24));
        entity.MarkAsUsed();

        // Assert
        entity.IsValid().Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenExpired_ReturnsFalse()
    {
        // Arrange — builder seeds ExpiresAt in the past
        var entity = new ActivationTokenBuilder()
            .WithExpiresAt(DateTime.UtcNow.AddHours(-1))
            .Build();

        // Assert
        entity.IsValid().Should().BeFalse();
    }

    // ─── ComputeHash ─────────────────────────────────────────────────────────────

    [Fact]
    public void ComputeHash_WhenCalledWithSameInput_ReturnsSameHash()
    {
        // Arrange
        const string rawToken = "some_raw_token_value";

        // Act
        var hash1 = ActivationToken.ComputeHash(rawToken);
        var hash2 = ActivationToken.ComputeHash(rawToken);

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void ComputeHash_WhenCalledWithDifferentInputs_ReturnsDifferentHashes()
    {
        // Arrange / Act
        var hash1 = ActivationToken.ComputeHash("token_a");
        var hash2 = ActivationToken.ComputeHash("token_b");

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void ComputeHash_WhenCalled_ReturnsLowercaseHex()
    {
        // Arrange / Act
        var hash = ActivationToken.ComputeHash("any_token");

        // Assert
        hash.Should().MatchRegex("^[0-9a-f]+$");
    }
}
