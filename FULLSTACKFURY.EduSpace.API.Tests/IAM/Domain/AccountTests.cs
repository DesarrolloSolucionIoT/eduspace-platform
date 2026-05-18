using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.ValueObjects;
using FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.IAM;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.IAM.Domain;

public class AccountTests
{
    // ─── Constructor ────────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WhenValidAdminData_SetsPropertiesCorrectly()
    {
        // Arrange
        const string username = "admin@example.com";
        const string passwordHash = "hashed_pwd";
        const string role = "RoleAdmin";

        // Act
        var account = new Account(username, passwordHash, role);

        // Assert
        account.Username.Should().Be(username);
        account.PasswordHash.Should().Be(passwordHash);
        account.Role.Should().Be(ERoles.RoleAdmin);
    }

    [Fact]
    public void Constructor_WhenValidTeacherData_SetsRoleTeacher()
    {
        // Arrange / Act
        var account = new Account("teacher@example.com", "hashed_pwd", "RoleTeacher");

        // Assert
        account.Role.Should().Be(ERoles.RoleTeacher);
    }

    [Theory]
    [InlineData("RoleAdmin")]
    [InlineData("RoleTeacher")]
    public void Constructor_WhenRoleIsValid_DoesNotThrow(string role)
    {
        // Arrange / Act
        Action act = () => new Account("user@example.com", "hash", role);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_WhenRoleIsInvalid_ThrowsArgumentException()
    {
        // Arrange / Act
        Action act = () => new Account("user@example.com", "hash", "RoleInvalid");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    // ─── UpdateUsername ──────────────────────────────────────────────────────────

    [Fact]
    public void UpdateUsername_WhenCalled_ChangesUsername()
    {
        // Arrange
        var account = new AccountBuilder().WithUsername("old@example.com").Build();

        // Act
        account.UpdateUsername("new@example.com");

        // Assert
        account.Username.Should().Be("new@example.com");
    }

    // ─── UpdatePasswordHash ──────────────────────────────────────────────────────

    [Fact]
    public void UpdatePasswordHash_WhenCalled_ChangesPasswordHash()
    {
        // Arrange
        var account = new AccountBuilder().WithPasswordHash("old_hash").Build();

        // Act
        account.UpdatePasswordHash("new_hash");

        // Assert
        account.PasswordHash.Should().Be("new_hash");
    }

    // ─── GetRole ─────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("RoleAdmin", "RoleAdmin")]
    [InlineData("RoleTeacher", "RoleTeacher")]
    public void GetRole_WhenCalled_ReturnsRoleAsString(string inputRole, string expected)
    {
        // Arrange
        var account = new AccountBuilder().WithRole(inputRole).Build();

        // Act
        var result = account.GetRole();

        // Assert
        result.Should().Be(expected);
    }
}
