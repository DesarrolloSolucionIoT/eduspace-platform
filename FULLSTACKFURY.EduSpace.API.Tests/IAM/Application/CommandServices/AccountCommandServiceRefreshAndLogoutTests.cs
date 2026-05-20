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

public class AccountCommandServiceRefreshAndLogoutTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();
    private readonly IActivationTokenRepository _activationTokenRepository = Substitute.For<IActivationTokenRepository>();
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
        _unitOfWork, _accountRepository, _activationTokenRepository,
        _tokenService, _hashingService, _emailService, _refreshTokenService,
        _teacherProfileRepository, _adminProfileRepository,
        _classroomQueryService, _meetingQueryService, _logger, NullLoggerFactory.Instance);

    // ─── RefreshAccessToken — happy path ────────────────────────────────────────

    [Fact]
    public async Task Handle_RefreshAccessToken_WhenTokenIsValid_ReturnsNewAccessAndRefreshTokens()
    {
        // Arrange
        const string oldRawToken = "old_refresh_token";
        const string newRaw = "new_refresh_token";
        const string newAccess = "new_access_token";

        var account = new AccountBuilder().AsAdmin().Build();
        var (oldEntity, _) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));

        var (newEntity1, _) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));
        _refreshTokenService.RotateAsync(oldRawToken)
            .Returns((newRaw, newEntity1, oldEntity));
        _accountRepository.FindByIdAsync(oldEntity.AccountId).Returns(account);
        _tokenService.GenerateToken(account).Returns(newAccess);

        var sut = CreateSut();

        // Act
        var (returnedAccessToken, returnedRefreshToken) =
            await sut.Handle(new RefreshAccessTokenCommand(oldRawToken));

        // Assert
        returnedAccessToken.Should().Be(newAccess);
        returnedRefreshToken.Should().Be(newRaw);
    }

    [Fact]
    public async Task Handle_RefreshAccessToken_WhenTokenIsValid_GeneratesNewAccessToken()
    {
        // Arrange
        const string oldRawToken = "old_token";
        const string newAccess = "fresh_access_token";
        var account = new AccountBuilder().AsAdmin().Build();
        var (oldEntity, _) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));

        var (newEntity2, _) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));
        _refreshTokenService.RotateAsync(oldRawToken).Returns(("new_raw", newEntity2, oldEntity));
        _accountRepository.FindByIdAsync(oldEntity.AccountId).Returns(account);
        _tokenService.GenerateToken(account).Returns(newAccess);

        var sut = CreateSut();

        // Act
        await sut.Handle(new RefreshAccessTokenCommand(oldRawToken));

        // Assert
        _tokenService.Received(1).GenerateToken(account);
    }

    // ─── RefreshAccessToken — token not found ───────────────────────────────────

    [Fact]
    public async Task Handle_RefreshAccessToken_WhenTokenNotFound_ThrowsRefreshTokenNotFoundException()
    {
        // Arrange
        const string rawToken = "nonexistent_token";
        _refreshTokenService.RotateAsync(rawToken).Returns<(string, RefreshToken, RefreshToken)>(
            _ => throw new RefreshTokenNotFoundException());

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new RefreshAccessTokenCommand(rawToken));

        // Assert
        await act.Should().ThrowAsync<RefreshTokenNotFoundException>();
    }

    // ─── RefreshAccessToken — token already used ────────────────────────────────

    [Fact]
    public async Task Handle_RefreshAccessToken_WhenTokenAlreadyUsed_ThrowsRefreshTokenAlreadyUsedException()
    {
        // Arrange
        const string rawToken = "used_token";
        _refreshTokenService.RotateAsync(rawToken).Returns<(string, RefreshToken, RefreshToken)>(
            _ => throw new RefreshTokenAlreadyUsedException());

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new RefreshAccessTokenCommand(rawToken));

        // Assert
        await act.Should().ThrowAsync<RefreshTokenAlreadyUsedException>();
    }

    // ─── RefreshAccessToken — token expired ─────────────────────────────────────

    [Fact]
    public async Task Handle_RefreshAccessToken_WhenTokenExpired_ThrowsRefreshTokenExpiredException()
    {
        // Arrange
        const string rawToken = "expired_token";
        _refreshTokenService.RotateAsync(rawToken).Returns<(string, RefreshToken, RefreshToken)>(
            _ => throw new RefreshTokenExpiredException());

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new RefreshAccessTokenCommand(rawToken));

        // Assert
        await act.Should().ThrowAsync<RefreshTokenExpiredException>();
    }

    // ─── RefreshAccessToken — account not found after rotation ──────────────────

    [Fact]
    public async Task Handle_RefreshAccessToken_WhenAccountNotFoundAfterRotation_ThrowsAccountNotFoundException()
    {
        // Arrange
        const string rawToken = "valid_raw_token";
        var (oldEntity, _) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));

        var (newEntityForNull, _) = RefreshToken.CreateNew(accountId: 1, lifetime: TimeSpan.FromDays(14));
        _refreshTokenService.RotateAsync(rawToken).Returns(("new_raw", newEntityForNull, oldEntity));
        _accountRepository.FindByIdAsync(oldEntity.AccountId).Returns((Account?)null);

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new RefreshAccessTokenCommand(rawToken));

        // Assert
        await act.Should().ThrowAsync<AccountNotFoundException>();
    }

    // ─── Logout — happy path ────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Logout_WhenCalled_RevokesRefreshToken()
    {
        // Arrange
        const string rawToken = "token_to_revoke";
        var sut = CreateSut();

        // Act
        await sut.Handle(new LogoutCommand(rawToken));

        // Assert
        await _refreshTokenService.Received(1).RevokeAsync(rawToken);
    }

    // ─── Logout — token not found ───────────────────────────────────────────────

    [Fact]
    public async Task Handle_Logout_WhenTokenNotFound_ThrowsRefreshTokenNotFoundException()
    {
        // Arrange
        const string rawToken = "unknown_token";
        _refreshTokenService.RevokeAsync(rawToken)
            .Returns(Task.FromException(new RefreshTokenNotFoundException()));

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new LogoutCommand(rawToken));

        // Assert
        await act.Should().ThrowAsync<RefreshTokenNotFoundException>();
    }
}
