using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;
using FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.Profiles;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.Profiles.Domain.Model.Aggregates;

public class TeacherProfileTests
{
    // -------------------------------------------------------------------------
    // Construction
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_WhenAllFieldsAreValid_ShouldCreateProfile()
    {
        // Arrange
        var accountId = ProfileTestBuilder.ValidAccountId();

        // Act
        var profile = new TeacherProfile("Carlos", "Mendoza", "c@edu.pe",
            "12345678", "Av. Lima 1", "912345678", accountId, administratorId: 10);

        // Assert
        profile.ProfileName.FirstName.Should().Be("Carlos");
        profile.ProfileName.LastName.Should().Be("Mendoza");
        profile.AdministratorId.Should().Be(10);
    }

    [Fact]
    public void Constructor_WhenCreatedFromCommand_ShouldMapAllFields()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidCreateTeacherCommand();
        var accountId = ProfileTestBuilder.ValidAccountId();

        // Act
        var profile = new TeacherProfile(command, accountId);

        // Assert
        profile.ProfileName.FirstName.Should().Be(command.FirstName);
        profile.ProfileName.LastName.Should().Be(command.LastName);
        profile.ProfilePrivateInformation.Email.Should().Be(command.Email);
        profile.AdministratorId.Should().Be(command.AdministratorId);
    }

    // -------------------------------------------------------------------------
    // Update
    // -------------------------------------------------------------------------

    [Fact]
    public void Update_WhenCommandIsValid_ShouldChangeProfileNameAndPrivateInformation()
    {
        // Arrange
        var profile = ProfileTestBuilder.ValidTeacherProfile();
        var command = ProfileTestBuilder.ValidUpdateTeacherCommand();

        // Act
        var result = profile.Update(command);

        // Assert
        result.ProfileName.LastName.Should().Be("Mendoza Updated");
    }

    [Fact]
    public void Update_WhenCalled_ShouldReturnSameInstance()
    {
        // Arrange
        var profile = ProfileTestBuilder.ValidTeacherProfile();
        var command = ProfileTestBuilder.ValidUpdateTeacherCommand();

        // Act
        var result = profile.Update(command);

        // Assert
        result.Should().BeSameAs(profile);
    }

    [Fact]
    public void Update_WhenEmailChanges_ShouldReflectNewEmail()
    {
        // Arrange
        var profile = ProfileTestBuilder.ValidTeacherProfile();
        var command = new UpdateTeacherProfileCommand(
            Id: 1, FirstName: "Carlos", LastName: "Mendoza",
            Email: "new.email@edu.pe", Dni: "12345678",
            Address: "Av. Lima 1", Phone: "912345678");

        // Act
        profile.Update(command);

        // Assert
        profile.ProfileEmail.Should().Be("new.email@edu.pe");
    }

    // -------------------------------------------------------------------------
    // Computed properties
    // -------------------------------------------------------------------------

    [Fact]
    public void ProfileFullName_ShouldReturnFirstAndLastNameCombined()
    {
        // Arrange
        var profile = ProfileTestBuilder.ValidTeacherProfile();

        // Act
        var fullName = profile.ProfileFullName;

        // Assert
        fullName.Should().Contain("Carlos").And.Contain("Mendoza");
    }
}
