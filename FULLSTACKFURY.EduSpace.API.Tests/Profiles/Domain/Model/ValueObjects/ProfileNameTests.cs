using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.Profiles.Domain.Model.ValueObjects;

public class ProfileNameTests
{
    [Fact]
    public void Constructor_WhenBothNamesAreValid_ShouldSetProperties()
    {
        // Arrange
        var first = "Ana";
        var last = "García";

        // Act
        var name = new ProfileName(first, last);

        // Assert
        name.FirstName.Should().Be(first);
        name.LastName.Should().Be(last);
    }

    [Fact]
    public void FullName_WhenValid_ShouldReturnCombinedFirstAndLastName()
    {
        // Arrange
        var name = new ProfileName("Ana", "García");

        // Act
        var full = name.FullName;

        // Assert
        full.Should().Be("Ana García");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null!)]
    public void Constructor_WhenFirstNameIsNullOrWhitespace_ShouldThrowInvalidProfileDataException(string? firstName)
    {
        // Arrange / Act
        var act = () => new ProfileName(firstName!, "García");

        // Assert
        act.Should().Throw<InvalidProfileDataException>()
            .WithMessage("*First name is required*");
    }

    [Fact]
    public void Constructor_WhenFirstNameExceeds100Characters_ShouldThrowInvalidProfileDataException()
    {
        // Arrange
        var tooLong = new string('A', 101);

        // Act
        var act = () => new ProfileName(tooLong, "García");

        // Assert
        act.Should().Throw<InvalidProfileDataException>()
            .WithMessage("*100 characters*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null!)]
    public void Constructor_WhenLastNameIsNullOrWhitespace_ShouldThrowInvalidProfileDataException(string? lastName)
    {
        // Arrange / Act
        var act = () => new ProfileName("Ana", lastName!);

        // Assert
        act.Should().Throw<InvalidProfileDataException>()
            .WithMessage("*Last name is required*");
    }

    [Fact]
    public void Constructor_WhenLastNameExceeds100Characters_ShouldThrowInvalidProfileDataException()
    {
        // Arrange
        var tooLong = new string('Z', 101);

        // Act
        var act = () => new ProfileName("Ana", tooLong);

        // Assert
        act.Should().Throw<InvalidProfileDataException>()
            .WithMessage("*100 characters*");
    }

    [Fact]
    public void Constructor_WhenNamesAreExactly100Characters_ShouldSucceed()
    {
        // Arrange
        var exactly100 = new string('B', 100);

        // Act
        var act = () => new ProfileName(exactly100, exactly100);

        // Assert
        act.Should().NotThrow();
    }
}
