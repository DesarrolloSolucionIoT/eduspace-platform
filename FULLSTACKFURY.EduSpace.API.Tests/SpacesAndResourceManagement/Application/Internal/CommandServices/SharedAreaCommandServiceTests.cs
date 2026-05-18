using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.Internal.CommandServices;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.SpacesAndResourceManagement.Application.Internal.CommandServices;

public class SharedAreaCommandServiceTests
{
    private readonly ISharedAreaRepository _sharedAreaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly SharedAreaCommandService _sut;

    public SharedAreaCommandServiceTests()
    {
        _sharedAreaRepository = Substitute.For<ISharedAreaRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _sut = new SharedAreaCommandService(_sharedAreaRepository, _unitOfWork);
    }

    // ── Handle(CreateSharedAreaCommand) ──────────────────────────────────────────

    [Fact]
    public async Task Handle_CreateSharedArea_WhenNameAlreadyExists_ThrowsInvalidSharedAreaDataException()
    {
        // Arrange
        var command = new CreateSharedAreaCommand("Biblioteca", 200, "Sala de lectura");
        _sharedAreaRepository.ExistsByNameAsync(command.Name).Returns(true);

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<InvalidSharedAreaDataException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task Handle_CreateSharedArea_WithValidData_ReturnsCreatedSharedArea()
    {
        // Arrange
        var command = new CreateSharedAreaCommand("Biblioteca", 200, "Sala de lectura");
        _sharedAreaRepository.ExistsByNameAsync(command.Name).Returns(false);

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(command.Name);
        result.Capacity.Should().Be(command.Capacity);
        result.Description.Should().Be(command.Description);
    }

    [Fact]
    public async Task Handle_CreateSharedArea_WithValidData_PersistsAndCompletesUnitOfWork()
    {
        // Arrange
        var command = new CreateSharedAreaCommand("Biblioteca", 200, "Sala de lectura");
        _sharedAreaRepository.ExistsByNameAsync(command.Name).Returns(false);

        // Act
        await _sut.Handle(command);

        // Assert
        await _sharedAreaRepository.Received(1).AddAsync(Arg.Any<SharedArea>());
        await _unitOfWork.Received(1).CompleteAsync();
    }

    // ── Handle(DeleteSharedAreaCommand) ──────────────────────────────────────────

    [Fact]
    public async Task Handle_DeleteSharedArea_WhenNotFound_ThrowsSharedAreaNotFoundException()
    {
        // Arrange
        var command = new DeleteSharedAreaCommand(999);
        _sharedAreaRepository.FindByIdAsync(command.SharedAreaId).Returns((SharedArea?)null);

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<SharedAreaNotFoundException>();
    }

    [Fact]
    public async Task Handle_DeleteSharedArea_WhenFound_RemovesAndCompletesUnitOfWork()
    {
        // Arrange
        var existing = new SharedArea("Biblioteca", 200, "Sala de lectura");
        var command = new DeleteSharedAreaCommand(1);
        _sharedAreaRepository.FindByIdAsync(command.SharedAreaId).Returns(existing);

        // Act
        await _sut.Handle(command);

        // Assert
        _sharedAreaRepository.Received(1).Remove(existing);
        await _unitOfWork.Received(1).CompleteAsync();
    }

    // ── Handle(UpdateSharedAreaCommand) ──────────────────────────────────────────

    [Fact]
    public async Task Handle_UpdateSharedArea_WhenNotFound_ThrowsSharedAreaNotFoundException()
    {
        // Arrange
        var command = new UpdateSharedAreaCommand(999, "Auditorio", 500, "Auditorio central");
        _sharedAreaRepository.FindByIdAsync(command.Id).Returns((SharedArea?)null);

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<SharedAreaNotFoundException>();
    }

    [Fact]
    public async Task Handle_UpdateSharedArea_WithValidData_ReturnsUpdatedSharedArea()
    {
        // Arrange
        var existing = new SharedArea("Biblioteca", 200, "Sala de lectura");
        var command = new UpdateSharedAreaCommand(1, "Auditorio", 500, "Auditorio central");
        _sharedAreaRepository.FindByIdAsync(command.Id).Returns(existing);

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(command.Name);
        result.Capacity.Should().Be(command.Capacity);
        result.Description.Should().Be(command.Description);
    }

    [Fact]
    public async Task Handle_UpdateSharedArea_WithValidData_UpdatesRepositoryAndCompletesUnitOfWork()
    {
        // Arrange
        var existing = new SharedArea("Biblioteca", 200, "Sala de lectura");
        var command = new UpdateSharedAreaCommand(1, "Auditorio", 500, "Auditorio central");
        _sharedAreaRepository.FindByIdAsync(command.Id).Returns(existing);

        // Act
        await _sut.Handle(command);

        // Assert
        _sharedAreaRepository.Received(1).Update(existing);
        await _unitOfWork.Received(1).CompleteAsync();
    }
}
