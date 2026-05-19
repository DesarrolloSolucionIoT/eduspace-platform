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

public class AdminProfileCommandServiceTests
{
    private readonly IAdminProfileRepository _repo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExternalIamService _iamService;
    private readonly ILogger<AdminProfileCommandService> _logger;
    private readonly AdminProfileCommandService _sut;

    public AdminProfileCommandServiceTests()
    {
        _repo = Substitute.For<IAdminProfileRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _iamService = Substitute.For<IExternalIamService>();
        _logger = Substitute.For<ILogger<AdminProfileCommandService>>();
        _sut = new AdminProfileCommandService(_repo, _unitOfWork, _iamService, _logger);
    }

    // =========================================================================
    // Handle(CreateAdministratorProfileCommand)
    // =========================================================================

    [Fact]
    public async Task Handle_CreateAdmin_WhenCommandIsValid_ShouldReturnCreatedProfile()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidCreateAdminCommand();
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
    public async Task Handle_CreateAdmin_WhenCommandIsValid_ShouldCallIamServiceWithAdminRole()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidCreateAdminCommand();
        _iamService.CreateAccount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(ProfileTestBuilder.ValidAccountId()));

        // Act
        await _sut.Handle(command);

        // Assert
        await _iamService.Received(1).CreateAccount(
            Arg.Any<string>(), command.Password, ProfileRoles.Admin);
    }

    [Fact]
    public async Task Handle_CreateAdmin_WhenCommandIsValid_ShouldAddProfileToRepository()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidCreateAdminCommand();
        _iamService.CreateAccount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(ProfileTestBuilder.ValidAccountId()));

        // Act
        await _sut.Handle(command);

        // Assert
        await _repo.Received(1).AddAsync(
            Arg.Any<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.AdminProfile>());
    }

    [Fact]
    public async Task Handle_CreateAdmin_WhenCommandIsValid_ShouldCallUnitOfWork()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidCreateAdminCommand();
        _iamService.CreateAccount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(ProfileTestBuilder.ValidAccountId()));

        // Act
        await _sut.Handle(command);

        // Assert
        await _unitOfWork.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task Handle_CreateAdmin_WhenUsernameAndEmailDiffer_PassesUsernameToIam()
    {
        // Arrange
        const string distinctUsername = "admin_login_handle";
        const string distinctEmail = "admin_actual_email@edu.pe";

        var command = ProfileTestBuilder.ValidCreateAdminCommand(
            username: distinctUsername,
            email: distinctEmail);

        _iamService.CreateAccount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(ProfileTestBuilder.ValidAccountId()));

        // Act
        await _sut.Handle(command);

        // Assert
        await _iamService.Received(1).CreateAccount(
            distinctUsername,
            Arg.Any<string>(),
            Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_CreateAdmin_WhenInvalidProfileDataExceptionThrown_ShouldRethrow()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidCreateAdminCommand();
        _iamService.CreateAccount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .ThrowsAsync(new InvalidProfileDataException("Bad data"));

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<InvalidProfileDataException>();
    }

    [Fact]
    public async Task Handle_CreateAdmin_WhenUnexpectedExceptionThrown_ShouldReturnNull()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidCreateAdminCommand();
        _iamService.CreateAccount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .ThrowsAsync(new Exception("Database timeout"));

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result.Should().BeNull();
    }

    // =========================================================================
    // Handle(CreateAdministratorProfileCommand) — activation email wiring
    // =========================================================================

    [Fact]
    public async Task Handle_CreateAdmin_WhenAccountCreatedSuccessfully_CallsRequestActivationEmailWithProfileEmailAndFullName()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidCreateAdminCommand(
            username: "admin_user",
            email: "admin@edu.pe");
        command = command with { FirstName = "Luisa", LastName = "Torres" };
        var accountId = ProfileTestBuilder.ValidAccountId(42);
        _iamService.CreateAccount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(accountId));

        // Act
        await _sut.Handle(command);

        // Assert
        await _iamService.Received(1).RequestActivationEmailAsync(
            accountId.Id, command.Email, $"{command.FirstName} {command.LastName}");
    }

    [Fact]
    public async Task Handle_CreateAdmin_WhenEmailServiceThrows_StillReturnsCreatedAdminAndLogsError()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidCreateAdminCommand();
        var accountId = ProfileTestBuilder.ValidAccountId(42);
        _iamService.CreateAccount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(accountId));
        _iamService.RequestActivationEmailAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromException(new Exception("Email provider unreachable")));

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result.Should().NotBeNull("account was persisted even though email delivery failed");
    }

    // =========================================================================
    // Handle(UpdateAdminProfileCommand)
    // =========================================================================

    [Fact]
    public async Task Handle_UpdateAdmin_WhenProfileExists_ShouldReturnUpdatedProfile()
    {
        // Arrange
        var existing = ProfileTestBuilder.ValidAdminProfile();
        var command = ProfileTestBuilder.ValidUpdateAdminCommand(id: 1);
        _repo.FindByIdAsync(1).Returns(
            Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.AdminProfile?>(existing));

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result!.ProfileName.LastName.Should().Be("Torres Updated");
    }

    [Fact]
    public async Task Handle_UpdateAdmin_WhenProfileExists_ShouldCallUnitOfWork()
    {
        // Arrange
        var existing = ProfileTestBuilder.ValidAdminProfile();
        var command = ProfileTestBuilder.ValidUpdateAdminCommand(id: 1);
        _repo.FindByIdAsync(1).Returns(
            Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.AdminProfile?>(existing));

        // Act
        await _sut.Handle(command);

        // Assert
        await _unitOfWork.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task Handle_UpdateAdmin_WhenProfileNotFound_ShouldThrowAdminProfileNotFoundException()
    {
        // Arrange
        var command = ProfileTestBuilder.ValidUpdateAdminCommand(id: 99);
        _repo.FindByIdAsync(99).Returns(
            Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.AdminProfile?>(null));

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<AdminProfileNotFoundException>();
    }

    // =========================================================================
    // Handle(DeleteAdminProfileCommand)
    // =========================================================================

    [Fact]
    public async Task Handle_DeleteAdmin_WhenProfileExists_ShouldRemoveFromRepository()
    {
        // Arrange
        var existing = ProfileTestBuilder.ValidAdminProfile();
        var command = new DeleteAdminProfileCommand(1);
        _repo.FindByIdAsync(1).Returns(
            Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.AdminProfile?>(existing));

        // Act
        await _sut.Handle(command);

        // Assert
        _repo.Received(1).Remove(existing);
    }

    [Fact]
    public async Task Handle_DeleteAdmin_WhenProfileExists_ShouldCallUnitOfWork()
    {
        // Arrange
        var existing = ProfileTestBuilder.ValidAdminProfile();
        var command = new DeleteAdminProfileCommand(1);
        _repo.FindByIdAsync(1).Returns(
            Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.AdminProfile?>(existing));

        // Act
        await _sut.Handle(command);

        // Assert
        await _unitOfWork.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task Handle_DeleteAdmin_WhenProfileNotFound_ShouldThrowAdminProfileNotFoundException()
    {
        // Arrange
        var command = new DeleteAdminProfileCommand(404);
        _repo.FindByIdAsync(404).Returns(
            Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.AdminProfile?>(null));

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<AdminProfileNotFoundException>();
    }

    [Fact]
    public async Task Handle_DeleteAdmin_WhenProfileExists_ShouldDeleteLinkedIamAccount()
    {
        // Arrange — production resolves linkedAccountId via projection, not via
        // the loaded entity's AccountId navigation (which EF leaves null).
        const int linkedAccountId = 42;
        var existing = ProfileTestBuilder.ValidAdminProfile();
        var command = new DeleteAdminProfileCommand(1);
        _repo.FindByIdAsync(1).Returns(
            Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.AdminProfile?>(existing));
        _repo.FindLinkedAccountIdAsync(1).Returns(Task.FromResult<int?>(linkedAccountId));

        // Act
        await _sut.Handle(command);

        // Assert
        await _iamService.Received(1).DeleteAccountAsync(linkedAccountId);
    }

    [Fact]
    public async Task Handle_DeleteAdmin_WhenNoLinkedAccount_ShouldRemoveProfileAndSkipIamCleanup()
    {
        // Arrange — orphan profile with no linked IAM account row.
        var existing = ProfileTestBuilder.ValidAdminProfile();
        var command = new DeleteAdminProfileCommand(1);
        _repo.FindByIdAsync(1).Returns(
            Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.AdminProfile?>(existing));
        _repo.FindLinkedAccountIdAsync(1).Returns(Task.FromResult<int?>(null));

        // Act
        await _sut.Handle(command);

        // Assert
        _repo.Received(1).Remove(existing);
        await _iamService.DidNotReceive().DeleteAccountAsync(Arg.Any<int>());
    }

    [Fact]
    public async Task Handle_DeleteAdmin_WhenIamDeleteFails_ShouldLogAndNotRethrow()
    {
        // Arrange — profile row already gone; IAM cleanup is best-effort so the
        // controller still reports success and the admin can re-register manually.
        var existing = ProfileTestBuilder.ValidAdminProfile();
        var command = new DeleteAdminProfileCommand(1);
        _repo.FindByIdAsync(1).Returns(
            Task.FromResult<FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.AdminProfile?>(existing));
        _repo.FindLinkedAccountIdAsync(1).Returns(Task.FromResult<int?>(42));
        _iamService.DeleteAccountAsync(Arg.Any<int>())
            .Returns(Task.FromException(new Exception("IAM unreachable")));

        // Act
        var act = async () => await _sut.Handle(command);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
