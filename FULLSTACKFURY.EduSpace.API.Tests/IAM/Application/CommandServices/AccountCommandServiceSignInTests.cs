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
using FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.IAM;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.IAM.Application.CommandServices;

public class AccountCommandServiceSignInTests
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

    // ─── SignIn — happy path (admin) ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_SignIn_WhenValidCredentialsAndAdminProfile_SendsVerificationEmail()
    {
        // Arrange
        const string username = "admin@example.com";
        const string password = "secret";
        var account = new AccountBuilder().WithUsername(username).AsAdmin().Build();
        var adminProfile = ProfileTestHelper.CreateAdminProfile(email: "admin@example.com");

        _accountRepository.FindByUsername(username).Returns(account);
        _hashingService.VerifyPassword(password, account.PasswordHash).Returns(true);
        _teacherProfileRepository.FindByAccountIdAsync(account.Id).Returns((global::FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.TeacherProfile?)null);
        _adminProfileRepository.FindByAccountIdAsync(account.Id).Returns(adminProfile);

        var sut = CreateSut();

        // Act
        await sut.Handle(new SignInCommand(username, password));

        // Assert
        await _emailService.Received(1).SendEmailAsync(
            "admin@example.com",
            Arg.Any<string>(),
            Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_SignIn_WhenValidCredentialsAndAdminProfile_PersistsVerificationCode()
    {
        // Arrange
        const string username = "admin@example.com";
        const string password = "secret";
        var account = new AccountBuilder().WithUsername(username).AsAdmin().Build();
        var adminProfile = ProfileTestHelper.CreateAdminProfile(email: "admin@example.com");

        _accountRepository.FindByUsername(username).Returns(account);
        _hashingService.VerifyPassword(password, account.PasswordHash).Returns(true);
        _teacherProfileRepository.FindByAccountIdAsync(account.Id).Returns((global::FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.TeacherProfile?)null);
        _adminProfileRepository.FindByAccountIdAsync(account.Id).Returns(adminProfile);

        var sut = CreateSut();

        // Act
        await sut.Handle(new SignInCommand(username, password));

        // Assert
        await _verificationCodeRepository.Received(1).AddAsync(Arg.Any<VerificationCode>());
        await _unitOfWork.Received().CompleteAsync();
    }

    [Fact]
    public async Task Handle_SignIn_WhenValidCredentialsAndTeacherProfile_SendsEmailToTeacher()
    {
        // Arrange
        const string username = "teacher@example.com";
        const string password = "secret";
        var account = new AccountBuilder().WithUsername(username).AsTeacher().Build();
        var teacherProfile = ProfileTestHelper.CreateTeacherProfile(email: "teacher@example.com");

        _accountRepository.FindByUsername(username).Returns(account);
        _hashingService.VerifyPassword(password, account.PasswordHash).Returns(true);
        _teacherProfileRepository.FindByAccountIdAsync(account.Id).Returns(teacherProfile);

        var sut = CreateSut();

        // Act
        await sut.Handle(new SignInCommand(username, password));

        // Assert
        await _emailService.Received(1).SendEmailAsync(
            "teacher@example.com",
            Arg.Any<string>(),
            Arg.Any<string>());
    }

    // ─── SignIn — account not found ──────────────────────────────────────────────

    [Fact]
    public async Task Handle_SignIn_WhenAccountNotFound_ThrowsInvalidCredentialsException()
    {
        // Arrange
        const string username = "unknown@example.com";
        _accountRepository.FindByUsername(username).Returns((Account?)null);

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new SignInCommand(username, "password"));

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    // ─── SignIn — wrong password ─────────────────────────────────────────────────

    [Fact]
    public async Task Handle_SignIn_WhenPasswordDoesNotMatch_ThrowsInvalidCredentialsException()
    {
        // Arrange
        const string username = "admin@example.com";
        var account = new AccountBuilder().WithUsername(username).Build();

        _accountRepository.FindByUsername(username).Returns(account);
        _hashingService.VerifyPassword("wrong_password", account.PasswordHash).Returns(false);

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new SignInCommand(username, "wrong_password"));

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task Handle_SignIn_WhenPasswordDoesNotMatch_DoesNotSendEmail()
    {
        // Arrange
        const string username = "admin@example.com";
        var account = new AccountBuilder().WithUsername(username).Build();

        _accountRepository.FindByUsername(username).Returns(account);
        _hashingService.VerifyPassword(Arg.Any<string>(), account.PasswordHash).Returns(false);

        var sut = CreateSut();

        // Act
        try { await sut.Handle(new SignInCommand(username, "wrong")); } catch { /* expected */ }

        // Assert
        await _emailService.DidNotReceive().SendEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    // ─── SignIn — profile not found (no email) ───────────────────────────────────

    [Fact]
    public async Task Handle_SignIn_WhenNoProfileFound_ThrowsAccountNotFoundException()
    {
        // Arrange
        const string username = "admin@example.com";
        var account = new AccountBuilder().WithUsername(username).Build();

        _accountRepository.FindByUsername(username).Returns(account);
        _hashingService.VerifyPassword(Arg.Any<string>(), account.PasswordHash).Returns(true);
        _teacherProfileRepository.FindByAccountIdAsync(account.Id)
            .Returns((global::FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.TeacherProfile?)null);
        _adminProfileRepository.FindByAccountIdAsync(account.Id)
            .Returns((global::FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates.AdminProfile?)null);

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new SignInCommand(username, "password"));

        // Assert
        await act.Should().ThrowAsync<AccountNotFoundException>()
            .WithMessage("*profile*");
    }
}
