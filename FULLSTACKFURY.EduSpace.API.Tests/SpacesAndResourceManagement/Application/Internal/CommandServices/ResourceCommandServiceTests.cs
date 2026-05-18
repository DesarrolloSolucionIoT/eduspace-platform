using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.Internal.CommandServices;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Resource;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.SpacesAndResourceManagement.Application.Internal.CommandServices;

public class ResourceCommandServiceTests
{
    private readonly IClassroomRepository _classroomRepository;
    private readonly IResourceRepository _resourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ResourceCommandService _sut;

    public ResourceCommandServiceTests()
    {
        _classroomRepository = Substitute.For<IClassroomRepository>();
        _resourceRepository = Substitute.For<IResourceRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _sut = new ResourceCommandService(_classroomRepository, _resourceRepository, _unitOfWork);
    }

    // ── Handle(CreateResourceCommand) ───────────────────────────────────────────

    [Fact]
    public async Task Handle_CreateResource_WhenClassroomNotFound_ThrowsClassroomNotFoundException()
    {
        // Arrange
        var command = new CreateResourceCommand("Proyector", "Electronico", 999);
        _classroomRepository.FindByIdAsync(command.ClassroomId).Returns((Classroom?)null);

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<ClassroomNotFoundException>();
    }

    [Fact]
    public async Task Handle_CreateResource_WhenNameAlreadyExistsInClassroom_ThrowsInvalidResourceDataException()
    {
        // Arrange
        var existingClassroom = new Classroom("Aula 101", "Descripcion valida", 1);
        var command = new CreateResourceCommand("Proyector", "Electronico", 1);
        _classroomRepository.FindByIdAsync(command.ClassroomId).Returns(existingClassroom);
        _resourceRepository.ExistsByNameAndClassroomIdAsync(command.Name, command.ClassroomId).Returns(true);

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<InvalidResourceDataException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task Handle_CreateResource_WithValidData_ReturnsCreatedResource()
    {
        // Arrange
        var existingClassroom = new Classroom("Aula 101", "Descripcion valida", 1);
        var command = new CreateResourceCommand("Proyector", "Electronico", 1);
        _classroomRepository.FindByIdAsync(command.ClassroomId).Returns(existingClassroom);
        _resourceRepository.ExistsByNameAndClassroomIdAsync(command.Name, command.ClassroomId).Returns(false);

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(command.Name);
        result.KindOfResource.Should().Be(command.KindOfResource);
        result.ClassroomId.Should().Be(command.ClassroomId);
    }

    [Fact]
    public async Task Handle_CreateResource_WithValidData_PersistsAndCompletesUnitOfWork()
    {
        // Arrange
        var existingClassroom = new Classroom("Aula 101", "Descripcion valida", 1);
        var command = new CreateResourceCommand("Proyector", "Electronico", 1);
        _classroomRepository.FindByIdAsync(command.ClassroomId).Returns(existingClassroom);
        _resourceRepository.ExistsByNameAndClassroomIdAsync(command.Name, command.ClassroomId).Returns(false);

        // Act
        await _sut.Handle(command);

        // Assert
        await _resourceRepository.Received(1).AddAsync(Arg.Any<Resource>());
        await _unitOfWork.Received(1).CompleteAsync();
    }

    // ── Handle(DeleteResourceCommand) ───────────────────────────────────────────

    [Fact]
    public async Task Handle_DeleteResource_WhenNotFound_ThrowsResourceNotFoundException()
    {
        // Arrange
        var command = new DeleteResourceCommand(999);
        _resourceRepository.FindByIdAsync(command.ResourceId).Returns((Resource?)null);

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    [Fact]
    public async Task Handle_DeleteResource_WhenFound_RemovesAndCompletesUnitOfWork()
    {
        // Arrange
        var existing = new Resource("Proyector", "Electronico", 1);
        var command = new DeleteResourceCommand(1);
        _resourceRepository.FindByIdAsync(command.ResourceId).Returns(existing);

        // Act
        await _sut.Handle(command);

        // Assert
        _resourceRepository.Received(1).Remove(existing);
        await _unitOfWork.Received(1).CompleteAsync();
    }

    // ── Handle(UpdateResourceCommand) ───────────────────────────────────────────

    [Fact]
    public async Task Handle_UpdateResource_WhenNotFound_ThrowsResourceNotFoundException()
    {
        // Arrange
        var command = new UpdateResourceCommand(999, "Televisor", "Multimedia", 1);
        _resourceRepository.FindByIdAsync(command.Id).Returns((Resource?)null);

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    [Fact]
    public async Task Handle_UpdateResource_WithValidData_ReturnsUpdatedResource()
    {
        // Arrange
        var existing = new Resource("Proyector", "Electronico", 1);
        var command = new UpdateResourceCommand(1, "Televisor", "Multimedia", 2);
        _resourceRepository.FindByIdAsync(command.Id).Returns(existing);

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(command.Name);
        result.KindOfResource.Should().Be(command.KindOfResource);
        result.ClassroomId.Should().Be(command.ClassroomId);
    }

    [Fact]
    public async Task Handle_UpdateResource_WithValidData_UpdatesRepositoryAndCompletesUnitOfWork()
    {
        // Arrange
        var existing = new Resource("Proyector", "Electronico", 1);
        var command = new UpdateResourceCommand(1, "Televisor", "Multimedia", 2);
        _resourceRepository.FindByIdAsync(command.Id).Returns(existing);

        // Act
        await _sut.Handle(command);

        // Assert
        _resourceRepository.Received(1).Update(existing);
        await _unitOfWork.Received(1).CompleteAsync();
    }
}
