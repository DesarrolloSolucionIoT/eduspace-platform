using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.ValueObjects;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.BreakdownManagement.Domain.Model.ValueObjects;

public class ResourceIdTests
{
    [Fact]
    public void Constructor_ValidId_SetsIdProperty()
    {
        // Arrange
        const int validId = 5;

        // Act
        var resourceId = new ResourceId(validId);

        // Assert
        resourceId.Id.Should().Be(validId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public void Constructor_InvalidId_ThrowsArgumentException(int invalidId)
    {
        // Arrange & Act
        Action act = () => _ = new ResourceId(invalidId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Resource ID must be greater than 0*");
    }

    [Fact]
    public void TwoResourceIds_WithSameId_AreEqual()
    {
        // Arrange
        var first = new ResourceId(10);
        var second = new ResourceId(10);

        // Act & Assert
        first.Should().Be(second);
    }

    [Fact]
    public void TwoResourceIds_WithDifferentIds_AreNotEqual()
    {
        // Arrange
        var first = new ResourceId(1);
        var second = new ResourceId(2);

        // Act & Assert
        first.Should().NotBe(second);
    }
}
