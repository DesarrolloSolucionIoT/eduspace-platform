using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.QueryServices;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.Profiles;
using NSubstitute;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.Profiles.Application.Internal.QueryServices;

public class TeacherProfileQueryServiceTests
{
    private readonly ITeacherProfileRepository _repo;
    private readonly TeacherProfileQueryService _sut;

    public TeacherProfileQueryServiceTests()
    {
        _repo = Substitute.For<ITeacherProfileRepository>();
        _sut = new TeacherProfileQueryService(_repo);
    }

    // =========================================================================
    // Handle(GetTeacherProfileByIdQuery)
    // =========================================================================

    [Fact]
    public async Task Handle_GetById_WhenProfileExists_ShouldReturnProfile()
    {
        // Arrange
        var existing = ProfileTestBuilder.ValidTeacherProfile();
        var query = new GetTeacherProfileByIdQuery(1);
        _repo.FindByIdAsync(1).Returns(Task.FromResult<TeacherProfile?>(existing));

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(existing);
    }

    [Fact]
    public async Task Handle_GetById_WhenProfileNotFound_ShouldReturnNull()
    {
        // Arrange
        var query = new GetTeacherProfileByIdQuery(999);
        _repo.FindByIdAsync(999).Returns(Task.FromResult<TeacherProfile?>(null));

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetById_ShouldDelegateToRepository()
    {
        // Arrange
        var query = new GetTeacherProfileByIdQuery(7);
        _repo.FindByIdAsync(7).Returns(Task.FromResult<TeacherProfile?>(null));

        // Act
        await _sut.Handle(query);

        // Assert
        await _repo.Received(1).FindByIdAsync(7);
    }

    // =========================================================================
    // Handle(GetAllTeachersProfileQuery)
    // =========================================================================

    [Fact]
    public async Task Handle_GetAll_WhenNoProfilesExist_ShouldReturnEmptyCollection()
    {
        // Arrange
        var query = new GetAllTeachersProfileQuery();
        _repo.ListAsync().Returns(Task.FromResult<IEnumerable<TeacherProfile>>([]));

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_GetAll_WhenProfilesExist_ShouldReturnAllProfiles()
    {
        // Arrange
        var profiles = new List<TeacherProfile>
        {
            ProfileTestBuilder.ValidTeacherProfile(),
            ProfileTestBuilder.ValidTeacherProfile()
        };
        var query = new GetAllTeachersProfileQuery();
        _repo.ListAsync().Returns(Task.FromResult<IEnumerable<TeacherProfile>>(profiles));

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetAll_ShouldDelegateToRepository()
    {
        // Arrange
        var query = new GetAllTeachersProfileQuery();
        _repo.ListAsync().Returns(Task.FromResult<IEnumerable<TeacherProfile>>([]));

        // Act
        await _sut.Handle(query);

        // Assert
        await _repo.Received(1).ListAsync();
    }
}
