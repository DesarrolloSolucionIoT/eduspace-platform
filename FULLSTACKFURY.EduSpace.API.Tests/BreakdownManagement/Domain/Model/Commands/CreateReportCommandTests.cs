using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Exceptions;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.BreakdownManagement.Domain.Model.Commands;

public class CreateReportCommandTests
{
    [Fact]
    public void Constructor_ValidArguments_SetsPropertiesCorrectly()
    {
        // Arrange
        var createdAt = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var command = new CreateReportCommand("Fire hazard", "Smoke detected.", 5, createdAt);

        // Assert
        command.KindOfReport.Should().Be("Fire hazard");
        command.Description.Should().Be("Smoke detected.");
        command.ResourceId.Should().Be(5);
        command.CreatedAt.Should().Be(createdAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyOrNullKindOfReport_ThrowsInvalidReportDataException(string? kindOfReport)
    {
        // Arrange & Act
        Action act = () => _ = new CreateReportCommand(kindOfReport!, "Valid description.", 1, DateTime.UtcNow);

        // Assert
        act.Should().Throw<InvalidReportDataException>()
            .WithMessage("*KindOfReport cannot be null or empty*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyOrNullDescription_ThrowsInvalidReportDataException(string? description)
    {
        // Arrange & Act
        Action act = () => _ = new CreateReportCommand("Valid kind", description!, 1, DateTime.UtcNow);

        // Assert
        act.Should().Throw<InvalidReportDataException>()
            .WithMessage("*Description cannot be null or empty*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_InvalidResourceId_ThrowsInvalidReportDataException(int resourceId)
    {
        // Arrange & Act
        Action act = () => _ = new CreateReportCommand("Valid kind", "Valid description.", resourceId, DateTime.UtcNow);

        // Assert
        act.Should().Throw<InvalidReportDataException>()
            .WithMessage("*ResourceId must be greater than 0*");
    }
}
