using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.CommandServices;
using FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.OutboundServices.ACL;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Constants;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.Profiles;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.Profiles.Application.Internal.CommandServices;

public class TeacherProfileCommandServiceTests
{
    private readonly ITeacherProfileRepository _repo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExternalIamService _iamService;
    private readonly ILogger<TeacherProfileCommandService> _logger;
    private readonly TeacherProfileCommandService _sut;

    public TeacherProfileCommandServiceTests()
    {
        _repo = Substitute.For<ITeacherProfileRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _iamService = Substitute.For<IExternalIamService>();
        _logger = Substitute.For<ILogger<TeacherProfileCommandService>>();
        _sut = new TeacherProfileCommandService(_repo, _unitOfWork, _iamService, _logger);
    }

    // =========================================================================
    // Handle(CreateTeacherProfileCommand)
    // =========================================================================

    [Fact]
    public async Task Handle_CreateTeacher_WhenCommandIsValid_ShouldReturnCreatedProfile()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidCreateTeacherCommand();
        var accountId = ProfileTestBuilder.ValidAccountId();
        _iamService.CreateAccount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(accountId));

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result!.ProfileName.FirstName.Should().Be(command.FirstName);
    }

    [Fact]
    public async Task Handle_CreateTeacher_WhenCommandIsValid_ShouldCallIamServiceWithTeacherRole()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidCreateTeacherCommand();
        var accountId = ProfileTestBuilder.ValidAccountId();
        _iamService.CreateAccount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(accountId));

        // Act
        await _sut.Handle(command);

        // Assert
        await _iamService.Received(1).CreateAccount(
            Arg.Any<string>(), command.Password, ProfileRoles.Teacher);
    }

    [Fact]
    public async Task Handle_CreateTeacher_WhenCommandIsValid_ShouldAddProfileToRepository()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidCreateTeacherCommand();
        var accountId = ProfileTestBuilder.ValidAccountId();
        _iamService.CreateAccount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(accountId));

        // Act
        await _sut.Handle(command);

        // Assert
        await _repo.Received(1).AddAsync(Arg.Any<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.TeacherProfile>());
    }

    [Fact]
    public async Task Handle_CreateTeacher_WhenCommandIsValid_ShouldCallUnitOfWork()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidCreateTeacherCommand();
        _iamService.CreateAccount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(ProfileTestBuilder.ValidAccountId()));

        // Act
        await _sut.Handle(command);

        // Assert
        await _unitOfWork.Received(1).CompleteAsync();
    }

    /// <summary>
    /// Documents known bug: TeacherProfileCommandService passes command.Username (not command.Email)
    /// to IExternalIamService.CreateAccount as the username parameter — unlike AdminProfileCommandService.cs:22
    /// which incorrectly passes command.Email. This test locks the CURRENT correct behavior for TeacherProfile.
    /// </summary>
    [Fact]
    public async Task CreateTeacher_WhenUsernameAndEmailDiffer_PassesUsernameToIam_VerifiesCorrectBehavior()
    {
        // Arrange
        var distinctUsername = "teacher_login_handle";
        var distinctEmail = "teacher_actual_email@edu.pe";
        var command = ProfileTestBuilder.ValidCreateTeacherCommand(
            username: distinctUsername, email: distinctEmail);

        _iamService.CreateAccount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(ProfileTestBuilder.ValidAccountId()));

        // Act
        await _sut.Handle(command);

        // Assert
        // TeacherProfileCommandService correctly passes command.Username (not command.Email).
        await _iamService.Received(1).CreateAccount(
            distinctUsername, Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_CreateTeacher_WhenInvalidProfileDataExceptionThrown_ShouldRethrow()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidCreateTeacherCommand();
        _iamService.CreateAccount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .ThrowsAsync(new InvalidProfileDataException("Bad data"));

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<InvalidProfileDataException>();
    }

    [Fact]
    public async Task Handle_CreateTeacher_WhenUnexpectedExceptionThrown_ShouldReturnNull()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidCreateTeacherCommand();
        _iamService.CreateAccount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .ThrowsAsync(new Exception("Network failure"));

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result.Should().BeNull();
    }

    // =========================================================================
    // Handle(CreateTeacherProfileCommand) — auto-activation wiring
    // =========================================================================

    [Fact]
    public async Task Handle_CreateTeacher_WhenAccountCreated_CallsActivateAccountOnIamFacade()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidCreateTeacherCommand();
        var accountId = ProfileTestBuilder.ValidAccountId(55);
        _iamService.CreateAccount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(accountId));

        // Act
        await _sut.Handle(command);

        // Assert
        await _iamService.Received(1).ActivateAccountAsync(accountId.Id);
    }

    // =========================================================================
    // Handle(UpdateTeacherProfileCommand)
    // =========================================================================

    [Fact]
    public async Task Handle_UpdateTeacher_WhenProfileExists_ShouldReturnUpdatedProfile()
    {
        // Arrange
        var existing = ProfileTestBuilder.ValidTeacherProfile();
        var command = ProfileTestBuilder.ValidUpdateTeacherCommand(id: 1);
        _repo.FindByIdAsync(1).Returns(Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.TeacherProfile?>(existing));

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result!.ProfileName.LastName.Should().Be("Mendoza Updated");
    }

    [Fact]
    public async Task Handle_UpdateTeacher_WhenProfileExists_ShouldCallUnitOfWork()
    {
        // Arrange
        var existing = ProfileTestBuilder.ValidTeacherProfile();
        var command = ProfileTestBuilder.ValidUpdateTeacherCommand(id: 1);
        _repo.FindByIdAsync(1).Returns(Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.TeacherProfile?>(existing));

        // Act
        await _sut.Handle(command);

        // Assert
        await _unitOfWork.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task Handle_UpdateTeacher_WhenProfileNotFound_ShouldThrowTeacherProfileNotFoundException()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidUpdateTeacherCommand(id: 99);
        _repo.FindByIdAsync(99).Returns(Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.TeacherProfile?>(null));

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<TeacherProfileNotFoundException>();
    }

    // =========================================================================
    // Handle(DeleteTeacherProfileCommand)
    // =========================================================================

    [Fact]
    public async Task Handle_DeleteTeacher_WhenProfileExists_ShouldRemoveFromRepository()
    {
        // Arrange
        var existing = ProfileTestBuilder.ValidTeacherProfile();
        var command = new DeleteTeacherProfileCommand(1);
        _repo.FindByIdAsync(1).Returns(Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.TeacherProfile?>(existing));

        // Act
        await _sut.Handle(command);

        // Assert
        _repo.Received(1).Remove(existing);
    }

    [Fact]
    public async Task Handle_DeleteTeacher_WhenProfileExists_ShouldCallUnitOfWork()
    {
        // Arrange
        var existing = ProfileTestBuilder.ValidTeacherProfile();
        var command = new DeleteTeacherProfileCommand(1);
        _repo.FindByIdAsync(1).Returns(Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.TeacherProfile?>(existing));

        // Act
        await _sut.Handle(command);

        // Assert
        await _unitOfWork.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task Handle_DeleteTeacher_WhenProfileNotFound_ShouldThrowTeacherProfileNotFoundException()
    {
        // Arrange
        var command = new DeleteTeacherProfileCommand(404);
        _repo.FindByIdAsync(404).Returns(Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.TeacherProfile?>(null));

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<TeacherProfileNotFoundException>();
    }

    [Fact]
    public async Task Handle_DeleteTeacher_WhenProfileExists_ShouldDeleteLinkedIamAccount()
    {
        // Arrange — see AdminProfileCommandServiceTests for the projection rationale.
        const int linkedAccountId = 17;
        var existing = ProfileTestBuilder.ValidTeacherProfile();
        var command = new DeleteTeacherProfileCommand(1);
        _repo.FindByIdAsync(1).Returns(Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.TeacherProfile?>(existing));
        _repo.FindLinkedAccountIdAsync(1).Returns(Task.FromResult<int?>(linkedAccountId));

        // Act
        await _sut.Handle(command);

        // Assert
        await _iamService.Received(1).DeleteAccountAsync(linkedAccountId);
    }

    [Fact]
    public async Task Handle_DeleteTeacher_WhenNoLinkedAccount_ShouldRemoveProfileAndSkipIamCleanup()
    {
        // Arrange
        var existing = ProfileTestBuilder.ValidTeacherProfile();
        var command = new DeleteTeacherProfileCommand(1);
        _repo.FindByIdAsync(1).Returns(Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.TeacherProfile?>(existing));
        _repo.FindLinkedAccountIdAsync(1).Returns(Task.FromResult<int?>(null));

        // Act
        await _sut.Handle(command);

        // Assert
        _repo.Received(1).Remove(existing);
        await _iamService.DidNotReceive().DeleteAccountAsync(Arg.Any<int>());
    }

    [Fact]
    public async Task Handle_DeleteTeacher_WhenIamDeleteFails_ShouldLogAndNotRethrow()
    {
        // Arrange — best-effort cleanup, mirrors admin path so the controller still 200s.
        var existing = ProfileTestBuilder.ValidTeacherProfile();
        var command = new DeleteTeacherProfileCommand(1);
        _repo.FindByIdAsync(1).Returns(Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.TeacherProfile?>(existing));
        _repo.FindLinkedAccountIdAsync(1).Returns(Task.FromResult<int?>(17));
        _iamService.DeleteAccountAsync(Arg.Any<int>())
            .Returns(Task.FromException(new Exception("IAM unreachable")));

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
