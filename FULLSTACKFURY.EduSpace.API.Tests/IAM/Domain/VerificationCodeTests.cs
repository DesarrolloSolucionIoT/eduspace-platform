using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.IAM.Domain;

public class VerificationCodeTests
{
    // ─── Constructor ────────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WhenDefaultConstructorCalled_IsUsedIsFalse()
    {
        // Arrange / Act
        var code = new VerificationCode();

        // Assert
        code.IsUsed.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WhenPropertiesSet_ReflectsCorrectValues()
    {
        // Arrange
        var expiration = DateTime.UtcNow.AddMinutes(10);
        var account = new Account("user@example.com", "hash", "RoleAdmin");

        // Act
        var code = new VerificationCode
        {
            AccountId = 1,
            Code = "123456",
            ExpirationDate = expiration,
            Account = account
        };

        // Assert
        code.AccountId.Should().Be(1);
        code.Code.Should().Be("123456");
        code.ExpirationDate.Should().Be(expiration);
        code.IsUsed.Should().BeFalse();
    }

    // ─── MarkAsUsed ──────────────────────────────────────────────────────────────

    [Fact]
    public void MarkAsUsed_WhenCalled_SetsIsUsedToTrue()
    {
        // Arrange
        var code = new VerificationCode
        {
            AccountId = 1,
            Code = "654321",
            ExpirationDate = DateTime.UtcNow.AddMinutes(5),
            Account = new Account("user@example.com", "hash", "RoleAdmin")
        };

        // Act
        code.MarkAsUsed();

        // Assert
        code.IsUsed.Should().BeTrue();
    }

    [Fact]
    public void MarkAsUsed_WhenCalledTwice_RemainsTrue()
    {
        // Arrange
        var code = new VerificationCode
        {
            AccountId = 1,
            Code = "123456",
            ExpirationDate = DateTime.UtcNow.AddMinutes(5),
            Account = new Account("user@example.com", "hash", "RoleAdmin")
        };

        // Act
        code.MarkAsUsed();
        code.MarkAsUsed();

        // Assert
        code.IsUsed.Should().BeTrue();
    }
}
