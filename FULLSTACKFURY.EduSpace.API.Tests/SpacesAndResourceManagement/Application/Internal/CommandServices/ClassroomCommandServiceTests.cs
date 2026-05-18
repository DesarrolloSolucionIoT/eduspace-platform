using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.Internal.CommandServices;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.OutboundServices.ACL;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Classroom;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.SpacesAndResourceManagement.Application.Internal.CommandServices;

public class ClassroomCommandServiceTests
{
    private readonly IClassroomRepository _classroomRepository;
    private readonly IExternalProfileService _profileService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ClassroomCommandService _sut;

    public ClassroomCommandServiceTests()
    {
        _classroomRepository = Substitute.For<IClassroomRepository>();
        _profileService = Substitute.For<IExternalProfileService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _sut = new ClassroomCommandService(_classroomRepository, _profileService, _unitOfWork);
    }

    // ── Handle(CreateClassroomCommand) ───────────────────────────────────────────

    [Fact]
    public async Task Handle_CreateClassroom_WhenTeacherNotFound_ThrowsTeacherNotFoundForClassroomException()
    {
        // Arrange
        var command = new CreateClassroomCommand("Aula 101", "Descripcion valida", 99);
        _profileService.VerifyProfileAsync(command.TeacherId).Returns(false);

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<TeacherNotFoundForClassroomException>();
    }

    [Fact]
    public async Task Handle_CreateClassroom_WhenNameAlreadyExists_ThrowsInvalidClassroomDataException()
    {
        // Arrange
        var command = new CreateClassroomCommand("Aula Duplicada", "Descripcion valida", 1);
        _profileService.VerifyProfileAsync(command.TeacherId).Returns(true);
        _classroomRepository.ExistsByNameAsync(command.Name).Returns(true);

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<InvalidClassroomDataException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task Handle_CreateClassroom_WithValidData_ReturnsCreatedClassroom()
    {
        // Arrange
        var command = new CreateClassroomCommand("Aula 101", "Descripcion valida", 1);
        _profileService.VerifyProfileAsync(command.TeacherId).Returns(true);
        _classroomRepository.ExistsByNameAsync(command.Name).Returns(false);

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(command.Name);
        result.Description.Should().Be(command.Description);
    }

    [Fact]
    public async Task Handle_CreateClassroom_WithValidData_PersistsClassroomAndCompletesUnitOfWork()
    {
        // Arrange
        var command = new CreateClassroomCommand("Aula 101", "Descripcion valida", 1);
        _profileService.VerifyProfileAsync(command.TeacherId).Returns(true);
        _classroomRepository.ExistsByNameAsync(command.Name).Returns(false);

        // Act
        await _sut.Handle(command);

        // Assert
        await _classroomRepository.Received(1).AddAsync(Arg.Any<Classroom>());
        await _unitOfWork.Received(1).CompleteAsync();
    }

    // ── Handle(DeleteClassroomCommand) ───────────────────────────────────────────

    [Fact]
    public async Task Handle_DeleteClassroom_WhenClassroomNotFound_ThrowsClassroomNotFoundException()
    {
        // Arrange
        var command = new DeleteClassroomCommand(999);
        _classroomRepository.FindByIdAsync(command.ClassroomId).Returns((Classroom?)null);

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<ClassroomNotFoundException>();
    }

    [Fact]
    public async Task Handle_DeleteClassroom_WhenFound_RemovesAndCompletesUnitOfWork()
    {
        // Arrange
        var existing = new Classroom("Aula 101", "Descripcion valida", 1);
        var command = new DeleteClassroomCommand(1);
        _classroomRepository.FindByIdAsync(command.ClassroomId).Returns(existing);

        // Act
        await _sut.Handle(command);

        // Assert
        _classroomRepository.Received(1).Remove(existing);
        await _unitOfWork.Received(1).CompleteAsync();
    }

    // ── Handle(UpdateClassroomCommand) ───────────────────────────────────────────

    [Fact]
    public async Task Handle_UpdateClassroom_WhenClassroomNotFound_ThrowsClassroomNotFoundException()
    {
        // Arrange
        var command = new UpdateClassroomCommand(999, "Nuevo Nombre", "Nueva Desc", 2);
        _classroomRepository.FindByIdAsync(command.ClassroomId).Returns((Classroom?)null);

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<ClassroomNotFoundException>();
    }

    [Fact]
    public async Task Handle_UpdateClassroom_WhenTeacherNotFound_ThrowsTeacherNotFoundForClassroomException()
    {
        // Arrange
        var existing = new Classroom("Aula 101", "Descripcion original", 1);
        var command = new UpdateClassroomCommand(1, "Aula 202", "Nueva desc", 99);
        _classroomRepository.FindByIdAsync(command.ClassroomId).Returns(existing);
        _profileService.VerifyProfileAsync(command.TeacherId).Returns(false);

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<TeacherNotFoundForClassroomException>();
    }

    [Fact]
    public async Task Handle_UpdateClassroom_WithValidData_ReturnsUpdatedClassroom()
    {
        // Arrange
        var existing = new Classroom("Aula 101", "Descripcion original", 1);
        var command = new UpdateClassroomCommand(1, "Aula 202", "Descripcion actualizada", 2);
        _classroomRepository.FindByIdAsync(command.ClassroomId).Returns(existing);
        _profileService.VerifyProfileAsync(command.TeacherId).Returns(true);

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(command.Name);
        result.Description.Should().Be(command.Description);
    }

    [Fact]
    public async Task Handle_UpdateClassroom_WithValidData_UpdatesRepositoryAndCompletesUnitOfWork()
    {
        // Arrange
        var existing = new Classroom("Aula 101", "Descripcion original", 1);
        var command = new UpdateClassroomCommand(1, "Aula 202", "Descripcion actualizada", 2);
        _classroomRepository.FindByIdAsync(command.ClassroomId).Returns(existing);
        _profileService.VerifyProfileAsync(command.TeacherId).Returns(true);

        // Act
        await _sut.Handle(command);

        // Assert
        _classroomRepository.Received(1).Update(existing);
        await _unitOfWork.Received(1).CompleteAsync();
    }
}
