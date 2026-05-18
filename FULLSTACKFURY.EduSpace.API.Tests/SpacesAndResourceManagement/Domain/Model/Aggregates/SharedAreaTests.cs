using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.SpacesAndResourceManagement.Domain.Model.Aggregates;

public class SharedAreaTests
{
    // ── Constructor ─────────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidData_SetsPropertiesCorrectly()
    {
        // Arrange
        const string name = "Biblioteca";
        const int capacity = 200;
        const string description = "Sala de lectura principal";

        // Act
        var area = new SharedArea(name, capacity, description);

        // Assert
        area.Name.Should().Be(name);
        area.Capacity.Should().Be(capacity);
        area.Description.Should().Be(description);
    }

    [Fact]
    public void Constructor_FromCommand_SetsPropertiesCorrectly()
    {
        // Arrange
        var command = new CreateSharedAreaCommand("Auditorio", 500, "Auditorio principal");

        // Act
        var area = new SharedArea(command);

        // Assert
        area.Name.Should().Be(command.Name);
        area.Capacity.Should().Be(command.Capacity);
        area.Description.Should().Be(command.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null!)]
    public void Constructor_WithEmptyName_ThrowsInvalidSharedAreaDataException(string invalidName)
    {
        // Arrange / Act
        var act = () => new SharedArea(invalidName, 100, "Descripcion valida");

        // Assert
        act.Should().Throw<InvalidSharedAreaDataException>()
            .WithMessage("*name*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50)]
    public void Constructor_WithNonPositiveCapacity_ThrowsInvalidSharedAreaDataException(int invalidCapacity)
    {
        // Arrange / Act
        var act = () => new SharedArea("Biblioteca", invalidCapacity, "Descripcion valida");

        // Assert
        act.Should().Throw<InvalidSharedAreaDataException>()
            .WithMessage("*zero*");
    }

    [Fact]
    public void Constructor_WithCapacityExceedingMax_ThrowsInvalidSharedAreaDataException()
    {
        // Arrange / Act
        var act = () => new SharedArea("Biblioteca", 1001, "Descripcion valida");

        // Assert
        act.Should().Throw<InvalidSharedAreaDataException>()
            .WithMessage("*1000*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null!)]
    public void Constructor_WithEmptyDescription_ThrowsInvalidSharedAreaDataException(string invalidDescription)
    {
        // Arrange / Act
        var act = () => new SharedArea("Biblioteca", 100, invalidDescription);

        // Assert
        act.Should().Throw<InvalidSharedAreaDataException>()
            .WithMessage("*description*");
    }

    // ── Update ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Update_WithValidData_UpdatesAllProperties()
    {
        // Arrange
        var area = new SharedArea("Biblioteca", 100, "Descripcion original");

        // Act
        area.Update("Auditorio", 500, "Auditorio principal");

        // Assert
        area.Name.Should().Be("Auditorio");
        area.Capacity.Should().Be(500);
        area.Description.Should().Be("Auditorio principal");
    }

    [Fact]
    public void UpdateName_WithValidName_ChangesName()
    {
        // Arrange
        var area = new SharedArea("Biblioteca", 100, "Descripcion valida");

        // Act
        area.UpdateName("Sala de cómputo");

        // Assert
        area.Name.Should().Be("Sala de cómputo");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateName_WithEmptyName_ThrowsInvalidSharedAreaDataException(string badName)
    {
        // Arrange
        var area = new SharedArea("Biblioteca", 100, "Descripcion valida");

        // Act
        var act = () => area.UpdateName(badName);

        // Assert
        act.Should().Throw<InvalidSharedAreaDataException>();
    }

    [Fact]
    public void UpdateCapacity_WithValidCapacity_ChangesCapacity()
    {
        // Arrange
        var area = new SharedArea("Biblioteca", 100, "Descripcion valida");

        // Act
        area.UpdateCapacity(300);

        // Assert
        area.Capacity.Should().Be(300);
    }

    [Fact]
    public void UpdateCapacity_WithBoundaryMaxValue_Succeeds()
    {
        // Arrange
        var area = new SharedArea("Biblioteca", 100, "Descripcion valida");

        // Act
        area.UpdateCapacity(1000);

        // Assert
        area.Capacity.Should().Be(1000);
    }

    [Fact]
    public void UpdateCapacity_ExceedingMax_ThrowsInvalidSharedAreaDataException()
    {
        // Arrange
        var area = new SharedArea("Biblioteca", 100, "Descripcion valida");

        // Act
        var act = () => area.UpdateCapacity(1001);

        // Assert
        act.Should().Throw<InvalidSharedAreaDataException>();
    }

    [Fact]
    public void UpdateDescription_WithValidDescription_ChangesDescription()
    {
        // Arrange
        var area = new SharedArea("Biblioteca", 100, "Descripcion original");

        // Act
        area.UpdateDescription("Nueva descripcion");

        // Assert
        area.Description.Should().Be("Nueva descripcion");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDescription_WithEmptyDescription_ThrowsInvalidSharedAreaDataException(string badDesc)
    {
        // Arrange
        var area = new SharedArea("Biblioteca", 100, "Descripcion valida");

        // Act
        var act = () => area.UpdateDescription(badDesc);

        // Assert
        act.Should().Throw<InvalidSharedAreaDataException>();
    }
}
