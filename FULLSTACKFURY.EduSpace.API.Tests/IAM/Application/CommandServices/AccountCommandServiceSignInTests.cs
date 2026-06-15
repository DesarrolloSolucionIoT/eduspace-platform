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
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.IAM.Application.CommandServices;

public class AccountCommandServiceSignInTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();
    private readonly IActivationTokenRepository _activationTokenRepository = Substitute.For<IActivationTokenRepository>();
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository = Substitute.For<IPasswordResetTokenRepository>();
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
        _unitOfWork, _accountRepository, _activationTokenRepository, _passwordResetTokenRepository,
        _tokenService, _hashingService, _emailService, _refreshTokenService,
        _teacherProfileRepository, _adminProfileRepository,
        _classroomQueryService, _meetingQueryService, _logger, NullLoggerFactory.Instance);

    private Account BuildActiveAccount(string username, string role = "RoleAdmin")
    {
        var account = new AccountBuilder().WithUsername(username).WithRole(role).Build();
        account.Activate();
        return account;
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

    // ─── SignIn — inactive account ───────────────────────────────────────────────

    [Fact]
    public async Task Handle_SignIn_WhenAccountIsInactive_ThrowsAccountNotActivatedException()
    {
        // Arrange
        const string username = "admin@example.com";
        // Build an inactive account (IsActive defaults to false)
        var account = new AccountBuilder().WithUsername(username).AsAdmin().Build();

        _accountRepository.FindByUsername(username).Returns(account);
        _hashingService.VerifyPassword("correctpassword", account.PasswordHash).Returns(true);

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new SignInCommand(username, "correctpassword"));

        // Assert
        await act.Should().ThrowAsync<AccountNotActivatedException>();
    }

    [Fact]
    public async Task Handle_SignIn_WhenAccountIsInactive_DoesNotSendAnyEmail()
    {
        // Arrange
        const string username = "admin@example.com";
        var account = new AccountBuilder().WithUsername(username).AsAdmin().Build();

        _accountRepository.FindByUsername(username).Returns(account);
        _hashingService.VerifyPassword(Arg.Any<string>(), account.PasswordHash).Returns(true);

        var sut = CreateSut();

        // Act
        try { await sut.Handle(new SignInCommand(username, "pass")); } catch { /* expected */ }

        // Assert
        await _emailService.DidNotReceive().SendEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        await _emailService.DidNotReceive().SendActivationEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    // ─── SignIn — happy path (active admin) ──────────────────────────────────────

    [Fact]
    public async Task Handle_SignIn_WhenActiveAdminAccount_ReturnsJwtBundleDirectly()
    {
        // Arrange
        const string username = "admin@example.com";
        var account = BuildActiveAccount(username, "RoleAdmin");
        var adminProfile = ProfileTestHelper.CreateAdminProfile(email: username);

        _accountRepository.FindByUsername(username).Returns(account);
        _hashingService.VerifyPassword("correctpassword", account.PasswordHash).Returns(true);
        _tokenService.GenerateToken(account).Returns("access_token");
        var (refreshEntity, _) = RefreshToken.CreateNew(account.Id, TimeSpan.FromDays(14));
        _refreshTokenService.CreateForAccountAsync(account.Id).Returns(("raw_refresh", refreshEntity));
        _adminProfileRepository.FindByAccountIdAsync(account.Id).Returns(adminProfile);

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new SignInCommand(username, "correctpassword"));

        // Assert
        result.accessToken.Should().Be("access_token");
        result.refreshToken.Should().Be("raw_refresh");
        result.account.Should().BeSameAs(account);
    }

    [Fact]
    public async Task Handle_SignIn_WhenActiveAdminAccount_DoesNotSendAnyEmail()
    {
        // Arrange
        const string username = "admin@example.com";
        var account = BuildActiveAccount(username, "RoleAdmin");
        var adminProfile = ProfileTestHelper.CreateAdminProfile(email: username);

        _accountRepository.FindByUsername(username).Returns(account);
        _hashingService.VerifyPassword("correctpassword", account.PasswordHash).Returns(true);
        _tokenService.GenerateToken(account).Returns("access_token");
        var (refreshEntity, _) = RefreshToken.CreateNew(account.Id, TimeSpan.FromDays(14));
        _refreshTokenService.CreateForAccountAsync(account.Id).Returns(("raw_refresh", refreshEntity));
        _adminProfileRepository.FindByAccountIdAsync(account.Id).Returns(adminProfile);

        var sut = CreateSut();

        // Act
        await sut.Handle(new SignInCommand(username, "correctpassword"));

        // Assert
        await _emailService.DidNotReceive().SendEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        await _emailService.DidNotReceive().SendActivationEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_SignIn_WhenActiveAdminAccount_ReturnsAdminProfile()
    {
        // Arrange
        const string username = "admin@example.com";
        var account = BuildActiveAccount(username, "RoleAdmin");
        var adminProfile = ProfileTestHelper.CreateAdminProfile(email: username);

        _accountRepository.FindByUsername(username).Returns(account);
        _hashingService.VerifyPassword("pass", account.PasswordHash).Returns(true);
        _tokenService.GenerateToken(account).Returns("tok");
        var (refreshEntity, _) = RefreshToken.CreateNew(account.Id, TimeSpan.FromDays(14));
        _refreshTokenService.CreateForAccountAsync(account.Id).Returns(("ref", refreshEntity));
        _adminProfileRepository.FindByAccountIdAsync(account.Id).Returns(adminProfile);

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new SignInCommand(username, "pass"));

        // Assert
        result.adminProfile.Should().BeSameAs(adminProfile);
        result.teacherProfile.Should().BeNull();
    }

    // ─── SignIn — happy path (active teacher) ────────────────────────────────────

    [Fact]
    public async Task Handle_SignIn_WhenActiveTeacherAccount_ReturnsTeacherProfileAndClassrooms()
    {
        // Arrange
        const string username = "teacher@example.com";
        var account = BuildActiveAccount(username, "RoleTeacher");
        var teacherProfile = ProfileTestHelper.CreateTeacherProfile(email: username);

        _accountRepository.FindByUsername(username).Returns(account);
        _hashingService.VerifyPassword("pass", account.PasswordHash).Returns(true);
        _tokenService.GenerateToken(account).Returns("tok");
        var (refreshEntity, _) = RefreshToken.CreateNew(account.Id, TimeSpan.FromDays(14));
        _refreshTokenService.CreateForAccountAsync(account.Id).Returns(("ref", refreshEntity));
        _teacherProfileRepository.FindByAccountIdAsync(account.Id).Returns(teacherProfile);
        _classroomQueryService
            .Handle(Arg.Any<global::FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries.GetAllClassroomsByTeacherIdQuery>())
            .Returns(Enumerable.Empty<global::FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates.Classroom>());
        _meetingQueryService
            .Handle(Arg.Any<global::FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Queries.GetAllMeetingByTeacherIdQuery>())
            .Returns(Enumerable.Empty<global::FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates.Meeting>());

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new SignInCommand(username, "pass"));

        // Assert
        result.teacherProfile.Should().BeSameAs(teacherProfile);
        result.adminProfile.Should().BeNull();
        result.classrooms.Should().NotBeNull();
    }

    // ─── SignIn — email as identifier (fallback to profile email lookup) ─────────

    [Fact]
    public async Task Handle_SignIn_WhenInputIsEmail_AndUsernameLookupMisses_FallsBackToTeacherProfileEmail()
    {
        // Arrange
        const string email = "teacher@example.com";
        const string username = "teacher.one";
        var account = BuildActiveAccount(username, "RoleTeacher");
        var teacherProfile = ProfileTestHelper.CreateTeacherProfile(email: email);

        _accountRepository.FindByUsername(email).Returns((Account?)null);
        _teacherProfileRepository.FindAccountIdByEmailAsync(email).Returns(account.Id);
        _accountRepository.FindByIdAsync(account.Id).Returns(account);
        _hashingService.VerifyPassword("pass", account.PasswordHash).Returns(true);
        _tokenService.GenerateToken(account).Returns("tok");
        var (refreshEntity, _) = RefreshToken.CreateNew(account.Id, TimeSpan.FromDays(14));
        _refreshTokenService.CreateForAccountAsync(account.Id).Returns(("ref", refreshEntity));
        _teacherProfileRepository.FindByAccountIdAsync(account.Id).Returns(teacherProfile);
        _classroomQueryService
            .Handle(Arg.Any<global::FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries.GetAllClassroomsByTeacherIdQuery>())
            .Returns(Enumerable.Empty<global::FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates.Classroom>());
        _meetingQueryService
            .Handle(Arg.Any<global::FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Queries.GetAllMeetingByTeacherIdQuery>())
            .Returns(Enumerable.Empty<global::FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates.Meeting>());

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new SignInCommand(email, "pass"));

        // Assert
        result.accessToken.Should().Be("tok");
        result.account.Should().BeSameAs(account);
    }

    [Fact]
    public async Task Handle_SignIn_WhenInputIsEmail_AndTeacherMisses_FallsBackToAdminProfileEmail()
    {
        // Arrange
        const string email = "admin@example.com";
        const string username = "admin.one";
        var account = BuildActiveAccount(username, "RoleAdmin");
        var adminProfile = ProfileTestHelper.CreateAdminProfile(email: email);

        _accountRepository.FindByUsername(email).Returns((Account?)null);
        _teacherProfileRepository.FindAccountIdByEmailAsync(email).Returns((int?)null);
        _adminProfileRepository.FindAccountIdByEmailAsync(email).Returns(account.Id);
        _accountRepository.FindByIdAsync(account.Id).Returns(account);
        _hashingService.VerifyPassword("pass", account.PasswordHash).Returns(true);
        _tokenService.GenerateToken(account).Returns("tok");
        var (refreshEntity, _) = RefreshToken.CreateNew(account.Id, TimeSpan.FromDays(14));
        _refreshTokenService.CreateForAccountAsync(account.Id).Returns(("ref", refreshEntity));
        _adminProfileRepository.FindByAccountIdAsync(account.Id).Returns(adminProfile);

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new SignInCommand(email, "pass"));

        // Assert
        result.adminProfile.Should().BeSameAs(adminProfile);
    }

    [Fact]
    public async Task Handle_SignIn_WhenInputIsEmail_AndNoProfileMatches_ThrowsInvalidCredentialsException()
    {
        // Arrange
        const string email = "ghost@example.com";
        _accountRepository.FindByUsername(email).Returns((Account?)null);
        _teacherProfileRepository.FindAccountIdByEmailAsync(email).Returns((int?)null);
        _adminProfileRepository.FindAccountIdByEmailAsync(email).Returns((int?)null);

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new SignInCommand(email, "pass"));

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task Handle_SignIn_WhenInputHasNoAtSign_DoesNotInvokeEmailFallback()
    {
        // Arrange
        const string username = "no_at_sign_here";
        _accountRepository.FindByUsername(username).Returns((Account?)null);

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new SignInCommand(username, "pass"));

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
        await _teacherProfileRepository.DidNotReceive().FindAccountIdByEmailAsync(Arg.Any<string>());
        await _adminProfileRepository.DidNotReceive().FindAccountIdByEmailAsync(Arg.Any<string>());
    }
}
