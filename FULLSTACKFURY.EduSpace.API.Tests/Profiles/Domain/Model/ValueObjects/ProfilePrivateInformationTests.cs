using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.Profiles.Domain.Model.ValueObjects;

public class ProfilePrivateInformationTests
{
    private const string ValidEmail = "ana@edu.pe";
    private const string ValidDni = "12345678";
    private const string ValidAddress = "Av. Lima 100";
    private const string ValidPhone = "912345678";

    [Fact]
    public void Constructor_WhenAllFieldsAreValid_ShouldSetProperties()
    {
        // Arrange / Act
        var info = new ProfilePrivateInformation(ValidEmail, ValidDni, ValidAddress, ValidPhone);

        // Assert
        info.Email.Should().Be(ValidEmail);
        info.Dni.Should().Be(ValidDni);
        info.Address.Should().Be(ValidAddress);
        info.Phone.Should().Be(ValidPhone);
    }

    [Theory]
    [InlineData("")]
    [InlineData("notanemail")]
    [InlineData("missing@dot")]
    [InlineData("@nodomain.com")]
    public void Constructor_WhenEmailIsInvalid_ShouldThrowInvalidProfileDataException(string badEmail)
    {
        // Arrange / Act
        var act = () => new ProfilePrivateInformation(badEmail, ValidDni, ValidAddress, ValidPhone);

        // Assert
        act.Should().Throw<InvalidProfileDataException>()
            .WithMessage("*email*");
    }

    [Theory]
    [InlineData("1234567")]   // 7 digits
    [InlineData("123456789")] // 9 digits
    [InlineData("1234567A")]  // letter
    [InlineData("")]
    public void Constructor_WhenDniIsInvalid_ShouldThrowInvalidProfileDataException(string badDni)
    {
        // Arrange / Act
        var act = () => new ProfilePrivateInformation(ValidEmail, badDni, ValidAddress, ValidPhone);

        // Assert
        act.Should().Throw<InvalidProfileDataException>()
            .WithMessage("*DNI*8 digits*");
    }

    [Fact]
    public void Constructor_WhenAddressIsEmpty_ShouldThrowInvalidProfileDataException()
    {
        // Arrange / Act
        var act = () => new ProfilePrivateInformation(ValidEmail, ValidDni, "", ValidPhone);

        // Assert
        act.Should().Throw<InvalidProfileDataException>()
            .WithMessage("*Address is required*");
    }

    [Fact]
    public void Constructor_WhenAddressIsWhitespace_ShouldThrowInvalidProfileDataException()
    {
        // Arrange / Act
        var act = () => new ProfilePrivateInformation(ValidEmail, ValidDni, "   ", ValidPhone);

        // Assert
        act.Should().Throw<InvalidProfileDataException>()
            .WithMessage("*Address is required*");
    }

    [Theory]
    [InlineData("812345678")] // starts with 8
    [InlineData("91234567")]  // 8 digits
    [InlineData("9123456789")]// 10 digits
    [InlineData("")]
    public void Constructor_WhenPhoneIsInvalid_ShouldThrowInvalidProfileDataException(string badPhone)
    {
        // Arrange / Act
        var act = () => new ProfilePrivateInformation(ValidEmail, ValidDni, ValidAddress, badPhone);

        // Assert
        act.Should().Throw<InvalidProfileDataException>()
            .WithMessage("*Phone*9 digits*9*");
    }

    [Theory]
    [InlineData("900000000")]
    [InlineData("999999999")]
    [InlineData("912345678")]
    public void Constructor_WhenPhoneStartsWithNineAndHasNineDigits_ShouldSucceed(string validPhone)
    {
        // Arrange / Act
        var act = () => new ProfilePrivateInformation(ValidEmail, ValidDni, ValidAddress, validPhone);

        // Assert
        act.Should().NotThrow();
    }
}
