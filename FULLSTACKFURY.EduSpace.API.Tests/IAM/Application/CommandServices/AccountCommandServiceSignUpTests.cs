using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.CommandServices;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.IAM.Application.CommandServices;

public class AccountCommandServiceSignUpTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();
    private readonly IVerificationCodeRepository _verificationCodeRepository = Substitute.For<IVerificationCodeRepository>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IHashingService _hashingService = Substitute.For<IHashingService>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly IRefreshTokenService _refreshTokenService = Substitute.For<IRefreshTokenService>();
    private readonly ITeacherProfileRepository _teacherProfileRepository = Substitute.For<ITeacherProfileRepository>();
    private readonly IAdminProfileRepository _adminProfileRepository = Substitute.For<IAdminProfileRepository>();
    private readonly IClassroomQueryService _classroomQueryService = Substitute.For<IClassroomQueryService>();
    private readonly IMeetingQueryService _meetingQueryService = Substitute.For<IMeetingQueryService>();
    private readonly ILogger<AccountCommandService> _logger = Substitute.For<ILogger<AccountCommandService>>();

    private AccountCommandService CreateSut() => new(
        _unitOfWork, _accountRepository, _verificationCodeRepository,
        _tokenService, _hashingService, _emailService, _refreshTokenService,
        _teacherProfileRepository, _adminProfileRepository,
        _classroomQueryService, _meetingQueryService, _logger);

    // ─── SignUp — happy path ─────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_SignUp_WhenValidCommand_AddsAccountAndCompletes()
    {
        // Arrange
        var command = new SignUpCommand("admin@example.com", "password123", "RoleAdmin");
        _accountRepository.ExistsByUsername(command.Username).Returns(false);
        _hashingService.HashPassword(command.Password).Returns("hashed_password");

        var sut = CreateSut();

        // Act
        await sut.Handle(command);

        // Assert
        await _accountRepository.Received(1).AddAsync(Arg.Any<Account>());
        await _unitOfWork.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task Handle_SignUp_WhenValidCommand_HashesPasswordBeforeSaving()
    {
        // Arrange
        const string plainPassword = "plainPassword";
        const string hashedPassword = "hashed_result";
        var command = new SignUpCommand("admin@example.com", plainPassword, "RoleAdmin");
        _accountRepository.ExistsByUsername(command.Username).Returns(false);
        _hashingService.HashPassword(plainPassword).Returns(hashedPassword);

        var sut = CreateSut();

        // Act
        await sut.Handle(command);

        // Assert
        await _accountRepository.Received(1).AddAsync(
            Arg.Is<Account>(a => a.PasswordHash == hashedPassword));
    }

    // ─── SignUp — username already taken ────────────────────────────────────────

    [Fact]
    public async Task Handle_SignUp_WhenUsernameAlreadyTaken_ThrowsInvalidCredentialsException()
    {
        // Arrange
        var command = new SignUpCommand("existing@example.com", "password", "RoleAdmin");
        _accountRepository.ExistsByUsername(command.Username).Returns(true);

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task Handle_SignUp_WhenUsernameAlreadyTaken_DoesNotPersist()
    {
        // Arrange
        var command = new SignUpCommand("existing@example.com", "password", "RoleAdmin");
        _accountRepository.ExistsByUsername(command.Username).Returns(true);

        var sut = CreateSut();

        // Act
        try { await sut.Handle(command); } catch { /* expected */ }

        // Assert
        await _accountRepository.DidNotReceive().AddAsync(Arg.Any<Account>());
        await _unitOfWork.DidNotReceive().CompleteAsync();
    }

    // ─── SignUp — repository throws ──────────────────────────────────────────────

    [Fact]
    public async Task Handle_SignUp_WhenRepositoryThrows_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new SignUpCommand("admin@example.com", "password", "RoleAdmin");
        _accountRepository.ExistsByUsername(command.Username).Returns(false);
        _hashingService.HashPassword(command.Password).Returns("hashed");
        _accountRepository.AddAsync(Arg.Any<Account>())
            .Returns(Task.FromException(new Exception("DB failure")));

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*DB failure*");
    }
}
