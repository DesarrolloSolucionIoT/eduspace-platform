using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Classroom;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.ValueObjects;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.SpacesAndResourceManagement.Domain.Model.Aggregates;

public class ClassroomTests
{
    // ── Constructor ─────────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidData_SetsPropertiesCorrectly()
    {
        // Arrange
        const string name = "Aula 101";
        const string description = "Aula principal del primer piso";
        const int teacherId = 5;

        // Act
        var classroom = new Classroom(name, description, teacherId);

        // Assert
        classroom.Name.Should().Be(name);
        classroom.Description.Should().Be(description);
        classroom.TeacherId.Should().Be(new TeacherId(teacherId));
    }

    [Fact]
    public void Constructor_FromCommand_SetsPropertiesCorrectly()
    {
        // Arrange
        var command = new CreateClassroomCommand("Laboratorio", "Laboratorio de ciencias", 3);

        // Act
        var classroom = new Classroom(command);

        // Assert
        classroom.Name.Should().Be(command.Name);
        classroom.Description.Should().Be(command.Description);
        classroom.TeacherId.Should().Be(new TeacherId(command.TeacherId));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null!)]
    public void Constructor_WithEmptyName_ThrowsInvalidClassroomDataException(string invalidName)
    {
        // Arrange / Act
        var act = () => new Classroom(invalidName, "Descripcion valida", 1);

        // Assert
        act.Should().Throw<InvalidClassroomDataException>()
            .WithMessage("*name*");
    }

    [Fact]
    public void Constructor_WithNameExceeding100Chars_ThrowsInvalidClassroomDataException()
    {
        // Arrange
        var longName = new string('A', 101);

        // Act
        var act = () => new Classroom(longName, "Descripcion valida", 1);

        // Assert
        act.Should().Throw<InvalidClassroomDataException>()
            .WithMessage("*100*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null!)]
    public void Constructor_WithEmptyDescription_ThrowsInvalidClassroomDataException(string invalidDescription)
    {
        // Arrange / Act
        var act = () => new Classroom("Aula 101", invalidDescription, 1);

        // Assert
        act.Should().Throw<InvalidClassroomDataException>()
            .WithMessage("*description*");
    }

    [Fact]
    public void Constructor_WithDescriptionExceeding500Chars_ThrowsInvalidClassroomDataException()
    {
        // Arrange
        var longDescription = new string('D', 501);

        // Act
        var act = () => new Classroom("Aula 101", longDescription, 1);

        // Assert
        act.Should().Throw<InvalidClassroomDataException>()
            .WithMessage("*500*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithNonPositiveTeacherId_ThrowsInvalidClassroomDataException(int invalidTeacherId)
    {
        // Arrange / Act
        var act = () => new Classroom("Aula 101", "Descripcion valida", invalidTeacherId);

        // Assert
        act.Should().Throw<InvalidClassroomDataException>()
            .WithMessage("*TeacherId*");
    }

    // ── Update ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Update_WithValidData_UpdatesPropertiesCorrectly()
    {
        // Arrange
        var classroom = new Classroom("Aula 101", "Descripcion original", 1);

        // Act
        classroom.Update("Aula 202", "Descripcion actualizada", 2);

        // Assert
        classroom.Name.Should().Be("Aula 202");
        classroom.Description.Should().Be("Descripcion actualizada");
        classroom.TeacherId.Should().Be(new TeacherId(2));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithEmptyName_ThrowsInvalidClassroomDataException(string badName)
    {
        // Arrange
        var classroom = new Classroom("Aula 101", "Descripcion valida", 1);

        // Act
        var act = () => classroom.Update(badName, "Descripcion valida", 1);

        // Assert
        act.Should().Throw<InvalidClassroomDataException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Update_WithInvalidTeacherId_ThrowsInvalidClassroomDataException(int badId)
    {
        // Arrange
        var classroom = new Classroom("Aula 101", "Descripcion valida", 1);

        // Act
        var act = () => classroom.Update("Aula 101", "Descripcion valida", badId);

        // Assert
        act.Should().Throw<InvalidClassroomDataException>();
    }

    // ── Default collection ───────────────────────────────────────────────────────

    [Fact]
    public void NewClassroom_ResourcesCollection_IsEmpty()
    {
        // Arrange / Act
        var classroom = new Classroom("Aula 101", "Descripcion valida", 1);

        // Assert
        classroom.Resources.Should().BeEmpty();
    }
}
