using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.Profiles.Domain.Model.ValueObjects;

public class AccountIdTests
{
    [Fact]
    public void Constructor_WhenIdIsPositive_ShouldSetId()
    {
        // Arrange
        var id = 42;

        // Act
        var accountId = new AccountId(id);

        // Assert
        accountId.Id.Should().Be(id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WhenIdIsZeroOrNegative_ShouldThrowInvalidProfileDataException(int invalidId)
    {
        // Arrange / Act
        var act = () => new AccountId(invalidId);

        // Assert
        act.Should().Throw<InvalidProfileDataException>()
            .WithMessage("*greater than zero*");
    }

    [Fact]
    public void Constructor_WhenIdIsOne_ShouldBeValid()
    {
        // Arrange / Act
        var accountId = new AccountId(1);

        // Assert
        accountId.Id.Should().Be(1);
    }
}
