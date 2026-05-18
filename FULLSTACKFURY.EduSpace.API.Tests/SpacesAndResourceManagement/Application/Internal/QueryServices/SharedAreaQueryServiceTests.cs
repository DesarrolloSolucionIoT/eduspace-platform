using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.Internal.QueryServices;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.SpacesAndResourceManagement.Application.Internal.QueryServices;

public class SharedAreaQueryServiceTests
{
    private readonly ISharedAreaRepository _sharedAreaRepository;
    private readonly SharedAreaQueryService _sut;

    public SharedAreaQueryServiceTests()
    {
        _sharedAreaRepository = Substitute.For<ISharedAreaRepository>();
        _sut = new SharedAreaQueryService(_sharedAreaRepository);
    }

    // ── Handle(GetSharedAreaByIdQuery) ───────────────────────────────────────────

    [Fact]
    public async Task Handle_GetSharedAreaById_WhenFound_ReturnsSharedArea()
    {
        // Arrange
        var expected = new SharedArea("Biblioteca", 200, "Sala de lectura");
        var query = new GetSharedAreaByIdQuery(1);
        _sharedAreaRepository.FindByIdAsync(query.SharedAreaId).Returns(expected);

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task Handle_GetSharedAreaById_WhenNotFound_ReturnsNull()
    {
        // Arrange
        var query = new GetSharedAreaByIdQuery(999);
        _sharedAreaRepository.FindByIdAsync(query.SharedAreaId).Returns((SharedArea?)null);

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().BeNull();
    }

    // ── Handle(GetAllSharedAreasQuery) ───────────────────────────────────────────

    [Fact]
    public async Task Handle_GetAllSharedAreas_WhenEmpty_ReturnsEmptyCollection()
    {
        // Arrange
        var query = new GetAllSharedAreasQuery();
        _sharedAreaRepository.ListAsync().Returns(Enumerable.Empty<SharedArea>());

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_GetAllSharedAreas_WhenPopulated_ReturnsAllSharedAreas()
    {
        // Arrange
        var areas = new List<SharedArea>
        {
            new("Biblioteca", 200, "Sala de lectura"),
            new("Auditorio", 500, "Auditorio principal"),
            new("Laboratorio", 30, "Lab de ciencias")
        };
        var query = new GetAllSharedAreasQuery();
        _sharedAreaRepository.ListAsync().Returns(areas);

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().HaveCount(3);
    }
}
