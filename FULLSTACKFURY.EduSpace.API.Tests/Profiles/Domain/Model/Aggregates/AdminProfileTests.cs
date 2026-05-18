using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;
using FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.Profiles;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.Profiles.Domain.Model.Aggregates;

public class AdminProfileTests
{
    // -------------------------------------------------------------------------
    // Construction
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_WhenAllFieldsAreValid_ShouldCreateProfile()
    {
        // Arrange
        var accountId = ProfileTestBuilder.ValidAccountId(5);

        // Act
        var profile = new AdminProfile("Luisa", "Torres", "luisa@edu.pe",
            "87654321", "Jr. Cusco 200", "998877665", accountId);

        // Assert
        profile.ProfileName.FirstName.Should().Be("Luisa");
        profile.ProfileName.LastName.Should().Be("Torres");
        profile.ProfilePrivateInformation.Email.Should().Be("luisa@edu.pe");
    }

    [Fact]
    public void Constructor_WhenCreatedFromCommand_ShouldMapAllFields()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidCreateAdminCommand();
        var accountId = ProfileTestBuilder.ValidAccountId();

        // Act
        var profile = new AdminProfile(command, accountId);

        // Assert
        profile.ProfileName.FirstName.Should().Be(command.FirstName);
        profile.ProfileName.LastName.Should().Be(command.LastName);
        profile.ProfilePrivateInformation.Email.Should().Be(command.Email);
    }

    // -------------------------------------------------------------------------
    // Update
    // -------------------------------------------------------------------------

    [Fact]
    public void Update_WhenCommandIsValid_ShouldChangeProfileNameAndPrivateInformation()
    {
        // Arrange
        var profile = ProfileTestBuilder.ValidAdminProfile();
        var command = ProfileTestBuilder.ValidUpdateAdminCommand();

        // Act
        var result = profile.Update(command);

        // Assert
        result.ProfileName.LastName.Should().Be("Torres Updated");
    }

    [Fact]
    public void Update_WhenCalled_ShouldReturnSameInstance()
    {
        // Arrange
        var profile = ProfileTestBuilder.ValidAdminProfile();
        var command = ProfileTestBuilder.ValidUpdateAdminCommand();

        // Act
        var result = profile.Update(command);

        // Assert
        result.Should().BeSameAs(profile);
    }

    [Fact]
    public void Update_WhenEmailChanges_ShouldReflectNewEmail()
    {
        // Arrange
        var profile = ProfileTestBuilder.ValidAdminProfile();
        var command = new UpdateAdminProfileCommand(
            Id: 1, FirstName: "Luisa", LastName: "Torres",
            Email: "admin.new@edu.pe", Dni: "87654321",
            Address: "Av. Arequipa 789", Phone: "998877665");

        // Act
        profile.Update(command);

        // Assert
        profile.ProfileEmail.Should().Be("admin.new@edu.pe");
    }

    // -------------------------------------------------------------------------
    // Computed properties
    // -------------------------------------------------------------------------

    [Fact]
    public void ProfileFullName_ShouldReturnFirstAndLastNameCombined()
    {
        // Arrange
        var profile = ProfileTestBuilder.ValidAdminProfile();

        // Act
        var fullName = profile.ProfileFullName;

        // Assert
        fullName.Should().Contain("Luisa").And.Contain("Torres");
    }

    [Fact]
    public void ProfileDni_ShouldReturnDniFromPrivateInformation()
    {
        // Arrange
        var profile = ProfileTestBuilder.ValidAdminProfile();

        // Act
        var dni = profile.ProfileDni;

        // Assert
        dni.Should().Be("87654321");
    }
}
