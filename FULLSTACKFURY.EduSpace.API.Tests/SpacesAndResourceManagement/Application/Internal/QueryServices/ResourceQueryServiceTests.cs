using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.Internal.QueryServices;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.SpacesAndResourceManagement.Application.Internal.QueryServices;

public class ResourceQueryServiceTests
{
    private readonly IResourceRepository _resourceRepository;
    private readonly ResourceQueryService _sut;

    public ResourceQueryServiceTests()
    {
        _resourceRepository = Substitute.For<IResourceRepository>();
        _sut = new ResourceQueryService(_resourceRepository);
    }

    // ── Handle(GetResourceByIdQuery) ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_GetResourceById_WhenFound_ReturnsResource()
    {
        // Arrange
        var expected = new Resource("Proyector", "Electronico", 1);
        var query = new GetResourceByIdQuery(1);
        _resourceRepository.FindByIdAsync(query.ResourceId).Returns(expected);

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task Handle_GetResourceById_WhenNotFound_ReturnsNull()
    {
        // Arrange
        var query = new GetResourceByIdQuery(999);
        _resourceRepository.FindByIdAsync(query.ResourceId).Returns((Resource?)null);

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().BeNull();
    }

    // ── Handle(GetAllResourcesQuery) ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_GetAllResources_WhenEmpty_ReturnsEmptyCollection()
    {
        // Arrange
        var query = new GetAllResourcesQuery();
        _resourceRepository.ListAsync().Returns(Enumerable.Empty<Resource>());

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_GetAllResources_WhenPopulated_ReturnsAllResources()
    {
        // Arrange
        var resources = new List<Resource>
        {
            new("Proyector", "Electronico", 1),
            new("Pizarra", "Mobiliario", 1),
            new("Televisor", "Electronico", 2)
        };
        var query = new GetAllResourcesQuery();
        _resourceRepository.ListAsync().Returns(resources);

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().HaveCount(3);
    }

    // ── Handle(GetAllResourcesByClassroomIdQuery) ─────────────────────────────────

    [Fact]
    public async Task Handle_GetAllResourcesByClassroomId_WhenNoneFound_ReturnsEmptyCollection()
    {
        // Arrange
        var query = new GetAllResourcesByClassroomIdQuery(99);
        _resourceRepository.FindByClassroomIdAsync(query.ClassroomId).Returns(Enumerable.Empty<Resource>());

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_GetAllResourcesByClassroomId_WhenFound_ReturnsResourcesForClassroom()
    {
        // Arrange
        var resources = new List<Resource>
        {
            new("Proyector", "Electronico", 5),
            new("Pizarra", "Mobiliario", 5)
        };
        var query = new GetAllResourcesByClassroomIdQuery(5);
        _resourceRepository.FindByClassroomIdAsync(query.ClassroomId).Returns(resources);

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().HaveCount(2);
    }
}
