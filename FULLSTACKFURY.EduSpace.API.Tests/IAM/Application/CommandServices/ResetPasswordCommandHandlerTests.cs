using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.CommandServices;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.IAM;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.IAM.Application.CommandServices;

public class ResetPasswordCommandHandlerTests
{
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository =
        Substitute.For<IPasswordResetTokenRepository>();
    private readonly IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();
    private readonly IHashingService _hashingService = Substitute.For<IHashingService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILogger<ResetPasswordCommandHandler> _logger =
        Substitute.For<ILogger<ResetPasswordCommandHandler>>();

    private ResetPasswordCommandHandler CreateSut() =>
        new(_passwordResetTokenRepository, _accountRepository, _hashingService, _unitOfWork, _logger);

    // ─── Happy path ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenValidToken_UpdatesAccountPasswordHashWithHashedNewPassword()
    {
        // Arrange
        var (tokenEntity, rawToken) = PasswordResetToken.CreateNew(1, TimeSpan.FromHours(1));
        var account = new AccountBuilder().WithPasswordHash("old_hash").AsAdmin().Build();

        _passwordResetTokenRepository
            .FindByHashAsync(PasswordResetToken.ComputeHash(rawToken))
            .Returns(tokenEntity);
        _accountRepository.FindByIdAsync(tokenEntity.AccountId).Returns(account);
        _hashingService.HashPassword("NewSecret123").Returns("new_hashed_password");

        var sut = CreateSut();

        // Act
        await sut.Handle(new ResetPasswordCommand(rawToken, "NewSecret123"));

        // Assert
        account.PasswordHash.Should().Be("new_hashed_password");
    }

    [Fact]
    public async Task Handle_WhenValidToken_MarksTokenAsUsed()
    {
        // Arrange
        var (tokenEntity, rawToken) = PasswordResetToken.CreateNew(1, TimeSpan.FromHours(1));
        var account = new AccountBuilder().Build();

        _passwordResetTokenRepository
            .FindByHashAsync(PasswordResetToken.ComputeHash(rawToken))
            .Returns(tokenEntity);
        _accountRepository.FindByIdAsync(tokenEntity.AccountId).Returns(account);
        _hashingService.HashPassword(Arg.Any<string>()).Returns("hashed");

        var sut = CreateSut();

        // Act
        await sut.Handle(new ResetPasswordCommand(rawToken, "NewSecret123"));

        // Assert
        tokenEntity.UsedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WhenValidToken_PersistsBothChangesInSingleUoWCall()
    {
        // Arrange
        var (tokenEntity, rawToken) = PasswordResetToken.CreateNew(1, TimeSpan.FromHours(1));
        var account = new AccountBuilder().Build();

        _passwordResetTokenRepository
            .FindByHashAsync(PasswordResetToken.ComputeHash(rawToken))
            .Returns(tokenEntity);
        _accountRepository.FindByIdAsync(tokenEntity.AccountId).Returns(account);
        _hashingService.HashPassword(Arg.Any<string>()).Returns("hashed");

        var sut = CreateSut();

        // Act
        await sut.Handle(new ResetPasswordCommand(rawToken, "NewSecret123"));

        // Assert — exactly one CompleteAsync call for both mutations
        await _unitOfWork.Received(1).CompleteAsync();
    }

    // ─── Unknown token ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenTokenHashNotFound_ThrowsInvalidPasswordResetTokenException()
    {
        // Arrange
        _passwordResetTokenRepository
            .FindByHashAsync(Arg.Any<string>())
            .Returns((PasswordResetToken?)null);

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new ResetPasswordCommand("nonexistentrawtoken", "NewSecret123"));

        // Assert
        await act.Should().ThrowAsync<InvalidPasswordResetTokenException>();
    }

    // ─── Already used token ────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenTokenAlreadyUsed_ThrowsPasswordResetTokenAlreadyUsedException()
    {
        // Arrange
        var (tokenEntity, rawToken) = PasswordResetToken.CreateNew(1, TimeSpan.FromHours(1));
        tokenEntity.MarkAsUsed();

        _passwordResetTokenRepository
            .FindByHashAsync(PasswordResetToken.ComputeHash(rawToken))
            .Returns(tokenEntity);

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new ResetPasswordCommand(rawToken, "NewSecret123"));

        // Assert
        await act.Should().ThrowAsync<PasswordResetTokenAlreadyUsedException>();
    }

    // ─── Expired token ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenTokenIsExpired_ThrowsPasswordResetTokenExpiredException()
    {
        // Arrange
        var (tokenEntity, rawToken) = PasswordResetToken.CreateNew(1, TimeSpan.FromHours(1));

        var prop = typeof(PasswordResetToken).GetProperty(nameof(PasswordResetToken.ExpiresAt));
        prop!.SetValue(tokenEntity, DateTime.UtcNow.AddHours(-1));

        _passwordResetTokenRepository
            .FindByHashAsync(PasswordResetToken.ComputeHash(rawToken))
            .Returns(tokenEntity);

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new ResetPasswordCommand(rawToken, "NewSecret123"));

        // Assert
        await act.Should().ThrowAsync<PasswordResetTokenExpiredException>();
    }
}
