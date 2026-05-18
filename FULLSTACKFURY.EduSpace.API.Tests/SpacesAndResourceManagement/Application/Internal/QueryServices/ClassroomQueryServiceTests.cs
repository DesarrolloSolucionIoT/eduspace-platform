using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.Internal.QueryServices;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.SpacesAndResourceManagement.Application.Internal.QueryServices;

public class ClassroomQueryServiceTests
{
    private readonly IClassroomRepository _classroomRepository;
    private readonly ClassroomQueryService _sut;

    public ClassroomQueryServiceTests()
    {
        _classroomRepository = Substitute.For<IClassroomRepository>();
        _sut = new ClassroomQueryService(_classroomRepository);
    }

    // ── Handle(GetClassroomByIdQuery) ────────────────────────────────────────────

    [Fact]
    public async Task Handle_GetClassroomById_WhenFound_ReturnsClassroom()
    {
        // Arrange
        var expected = new Classroom("Aula 101", "Descripcion valida", 1);
        var query = new GetClassroomByIdQuery(1);
        _classroomRepository.FindByIdAsync(query.ClassroomId).Returns(expected);

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task Handle_GetClassroomById_WhenNotFound_ReturnsNull()
    {
        // Arrange
        var query = new GetClassroomByIdQuery(999);
        _classroomRepository.FindByIdAsync(query.ClassroomId).Returns((Classroom?)null);

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().BeNull();
    }

    // ── Handle(GetAllClassroomsQuery) ────────────────────────────────────────────

    [Fact]
    public async Task Handle_GetAllClassrooms_WhenEmpty_ReturnsEmptyCollection()
    {
        // Arrange
        var query = new GetAllClassroomsQuery();
        _classroomRepository.ListAsync().Returns(Enumerable.Empty<Classroom>());

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_GetAllClassrooms_WhenPopulated_ReturnsAllClassrooms()
    {
        // Arrange
        var classrooms = new List<Classroom>
        {
            new("Aula 101", "Descripcion uno", 1),
            new("Aula 102", "Descripcion dos", 2)
        };
        var query = new GetAllClassroomsQuery();
        _classroomRepository.ListAsync().Returns(classrooms);

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().HaveCount(2);
    }

    // ── Handle(GetAllClassroomsByTeacherIdQuery) ──────────────────────────────────

    [Fact]
    public async Task Handle_GetAllClassroomsByTeacherId_WhenNoneFound_ReturnsEmptyCollection()
    {
        // Arrange
        var query = new GetAllClassroomsByTeacherIdQuery(99);
        _classroomRepository.FindByTeacherIdAsync(query.TeacherId).Returns(Enumerable.Empty<Classroom>());

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_GetAllClassroomsByTeacherId_WhenFound_ReturnsClassroomsForTeacher()
    {
        // Arrange
        var classrooms = new List<Classroom>
        {
            new("Aula 101", "Primera aula", 5),
            new("Aula 202", "Segunda aula", 5)
        };
        var query = new GetAllClassroomsByTeacherIdQuery(5);
        _classroomRepository.FindByTeacherIdAsync(query.TeacherId).Returns(classrooms);

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().HaveCount(2);
    }
}
