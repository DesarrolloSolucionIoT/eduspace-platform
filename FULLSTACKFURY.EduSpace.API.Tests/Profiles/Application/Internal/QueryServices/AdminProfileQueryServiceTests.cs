using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.QueryServices;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.Profiles;
using NSubstitute;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.Profiles.Application.Internal.QueryServices;

public class AdminProfileQueryServiceTests
{
    private readonly IAdminProfileRepository _repo;
    private readonly AdminProfileQueryService _sut;

    public AdminProfileQueryServiceTests()
    {
        _repo = Substitute.For<IAdminProfileRepository>();
        _sut = new AdminProfileQueryService(_repo);
    }

    // =========================================================================
    // Handle(GetAdministratorProfileByIdQuery)
    // =========================================================================

    [Fact]
    public async Task Handle_GetById_WhenProfileExists_ShouldReturnProfile()
    {
        // Arrange
        var existing = ProfileTestBuilder.ValidAdminProfile();
        var query = new GetAdministratorProfileByIdQuery(1);
        _repo.FindByIdAsync(1).Returns(Task.FromResult<AdminProfile?>(existing));

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
        var query = new GetAdministratorProfileByIdQuery(999);
        _repo.FindByIdAsync(999).Returns(Task.FromResult<AdminProfile?>(null));

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetById_ShouldDelegateToRepository()
    {
        // Arrange
        var query = new GetAdministratorProfileByIdQuery(5);
        _repo.FindByIdAsync(5).Returns(Task.FromResult<AdminProfile?>(null));

        // Act
        await _sut.Handle(query);

        // Assert
        await _repo.Received(1).FindByIdAsync(5);
    }

    // =========================================================================
    // Handle(GetAllAdministratorsProfileQuery)
    // =========================================================================

    [Fact]
    public async Task Handle_GetAll_WhenNoProfilesExist_ShouldReturnEmptyCollection()
    {
        // Arrange
        var query = new GetAllAdministratorsProfileQuery();
        _repo.ListAsync().Returns(Task.FromResult<IEnumerable<AdminProfile>>([]));

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_GetAll_WhenProfilesExist_ShouldReturnAllProfiles()
    {
        // Arrange
        var profiles = new List<AdminProfile>
        {
            ProfileTestBuilder.ValidAdminProfile(),
            ProfileTestBuilder.ValidAdminProfile()
        };
        var query = new GetAllAdministratorsProfileQuery();
        _repo.ListAsync().Returns(Task.FromResult<IEnumerable<AdminProfile>>(profiles));

        // Act
        var result = await _sut.Handle(query);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetAll_ShouldDelegateToRepository()
    {
        // Arrange
        var query = new GetAllAdministratorsProfileQuery();
        _repo.ListAsync().Returns(Task.FromResult<IEnumerable<AdminProfile>>([]));

        // Act
        await _sut.Handle(query);

        // Assert
        await _repo.Received(1).ListAsync();
    }
}
