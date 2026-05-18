using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Resource;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.SpacesAndResourceManagement.Domain.Model.Aggregates;

public class ResourceTests
{
    // ── Constructor ─────────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidData_SetsPropertiesCorrectly()
    {
        // Arrange
        const string name = "Proyector";
        const string kind = "Electronico";
        const int classroomId = 10;

        // Act
        var resource = new Resource(name, kind, classroomId);

        // Assert
        resource.Name.Should().Be(name);
        resource.KindOfResource.Should().Be(kind);
        resource.ClassroomId.Should().Be(classroomId);
    }

    [Fact]
    public void Constructor_FromCommand_SetsPropertiesCorrectly()
    {
        // Arrange
        var command = new CreateResourceCommand("Pizarra", "Mobiliario", 5);

        // Act
        var resource = new Resource(command);

        // Assert
        resource.Name.Should().Be(command.Name);
        resource.KindOfResource.Should().Be(command.KindOfResource);
        resource.ClassroomId.Should().Be(command.ClassroomId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null!)]
    public void Constructor_WithEmptyName_ThrowsInvalidResourceDataException(string invalidName)
    {
        // Arrange / Act
        var act = () => new Resource(invalidName, "Electronico", 1);

        // Assert
        act.Should().Throw<InvalidResourceDataException>()
            .WithMessage("*name*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null!)]
    public void Constructor_WithEmptyKindOfResource_ThrowsInvalidResourceDataException(string invalidKind)
    {
        // Arrange / Act
        var act = () => new Resource("Proyector", invalidKind, 1);

        // Assert
        act.Should().Throw<InvalidResourceDataException>()
            .WithMessage("*KindOfResource*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-99)]
    public void Constructor_WithNonPositiveClassroomId_ThrowsInvalidResourceDataException(int invalidClassroomId)
    {
        // Arrange / Act
        var act = () => new Resource("Proyector", "Electronico", invalidClassroomId);

        // Assert
        act.Should().Throw<InvalidResourceDataException>()
            .WithMessage("*ClassroomId*");
    }

    // ── UpdateName ───────────────────────────────────────────────────────────────

    [Fact]
    public void UpdateName_WithValidName_ChangesName()
    {
        // Arrange
        var resource = new Resource("Proyector", "Electronico", 1);

        // Act
        resource.UpdateName("Televisor");

        // Assert
        resource.Name.Should().Be("Televisor");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateName_WithEmptyName_ThrowsInvalidResourceDataException(string badName)
    {
        // Arrange
        var resource = new Resource("Proyector", "Electronico", 1);

        // Act
        var act = () => resource.UpdateName(badName);

        // Assert
        act.Should().Throw<InvalidResourceDataException>();
    }

    // ── UpdateKindOfResource ──────────────────────────────────────────────────────

    [Fact]
    public void UpdateKindOfResource_WithValidKind_ChangesKind()
    {
        // Arrange
        var resource = new Resource("Proyector", "Electronico", 1);

        // Act
        resource.UpdateKindOfResource("Multimedia");

        // Assert
        resource.KindOfResource.Should().Be("Multimedia");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateKindOfResource_WithEmptyKind_ThrowsInvalidResourceDataException(string badKind)
    {
        // Arrange
        var resource = new Resource("Proyector", "Electronico", 1);

        // Act
        var act = () => resource.UpdateKindOfResource(badKind);

        // Assert
        act.Should().Throw<InvalidResourceDataException>();
    }

    // ── UpdateClassroomId ──────────────────────────────────────────────────────────

    [Fact]
    public void UpdateClassroomId_WithDifferentValidId_ChangesClassroomId()
    {
        // Arrange
        var resource = new Resource("Proyector", "Electronico", 1);

        // Act
        resource.UpdateClassroomId(7);

        // Assert
        resource.ClassroomId.Should().Be(7);
    }

    [Fact]
    public void UpdateClassroomId_WithSameId_ClassroomIdUnchanged()
    {
        // Arrange
        var resource = new Resource("Proyector", "Electronico", 3);

        // Act
        resource.UpdateClassroomId(3);

        // Assert
        resource.ClassroomId.Should().Be(3);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UpdateClassroomId_WithNonPositiveId_ThrowsInvalidResourceDataException(int badId)
    {
        // Arrange
        var resource = new Resource("Proyector", "Electronico", 1);

        // Act
        var act = () => resource.UpdateClassroomId(badId);

        // Assert
        act.Should().Throw<InvalidResourceDataException>();
    }
}
