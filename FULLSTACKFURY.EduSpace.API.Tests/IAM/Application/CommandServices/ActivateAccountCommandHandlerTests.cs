using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.CommandServices;
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

public class ActivateAccountCommandHandlerTests
{
    private readonly IActivationTokenRepository _activationTokenRepository = Substitute.For<IActivationTokenRepository>();
    private readonly IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILogger<ActivateAccountCommandHandler> _logger =
        Substitute.For<ILogger<ActivateAccountCommandHandler>>();

    private ActivateAccountCommandHandler CreateSut() =>
        new(_activationTokenRepository, _accountRepository, _unitOfWork, _logger);

    // ─── Happy path ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenValidToken_ActivatesAccount()
    {
        // Arrange
        var (tokenEntity, rawToken) = ActivationToken.CreateNew(1, TimeSpan.FromHours(24));
        var account = new AccountBuilder().WithUsername("admin@example.com").AsAdmin().Build();

        _activationTokenRepository
            .FindByHashAsync(ActivationToken.ComputeHash(rawToken))
            .Returns(tokenEntity);
        _accountRepository.FindByIdAsync(tokenEntity.AccountId).Returns(account);

        var sut = CreateSut();

        // Act
        await sut.Handle(new ActivateAccountCommand(rawToken));

        // Assert
        account.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenValidToken_MarksTokenAsUsed()
    {
        // Arrange
        var (tokenEntity, rawToken) = ActivationToken.CreateNew(1, TimeSpan.FromHours(24));
        var account = new AccountBuilder().Build();

        _activationTokenRepository
            .FindByHashAsync(ActivationToken.ComputeHash(rawToken))
            .Returns(tokenEntity);
        _accountRepository.FindByIdAsync(tokenEntity.AccountId).Returns(account);

        var sut = CreateSut();

        // Act
        await sut.Handle(new ActivateAccountCommand(rawToken));

        // Assert
        tokenEntity.UsedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WhenValidToken_PersistsBothChangesInSingleUoWCall()
    {
        // Arrange
        var (tokenEntity, rawToken) = ActivationToken.CreateNew(1, TimeSpan.FromHours(24));
        var account = new AccountBuilder().Build();

        _activationTokenRepository
            .FindByHashAsync(ActivationToken.ComputeHash(rawToken))
            .Returns(tokenEntity);
        _accountRepository.FindByIdAsync(tokenEntity.AccountId).Returns(account);

        var sut = CreateSut();

        // Act
        await sut.Handle(new ActivateAccountCommand(rawToken));

        // Assert — exactly one CompleteAsync call for both mutations
        await _unitOfWork.Received(1).CompleteAsync();
    }

    // ─── Unknown token ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenTokenHashNotFound_ThrowsInvalidActivationTokenException()
    {
        // Arrange
        var rawToken = "nonexistentrawtoken";

        _activationTokenRepository
            .FindByHashAsync(Arg.Any<string>())
            .Returns((ActivationToken?)null);

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new ActivateAccountCommand(rawToken));

        // Assert
        await act.Should().ThrowAsync<InvalidActivationTokenException>();
    }

    // ─── Expired token ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenTokenIsExpired_ThrowsActivationTokenExpiredException()
    {
        // Arrange
        var (tokenEntity, rawToken) = ActivationToken.CreateNew(1, TimeSpan.FromHours(24));

        // Override ExpiresAt to the past using the builder approach
        var prop = typeof(ActivationToken).GetProperty(nameof(ActivationToken.ExpiresAt));
        prop!.SetValue(tokenEntity, DateTime.UtcNow.AddHours(-1));

        _activationTokenRepository
            .FindByHashAsync(ActivationToken.ComputeHash(rawToken))
            .Returns(tokenEntity);

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new ActivateAccountCommand(rawToken));

        // Assert
        await act.Should().ThrowAsync<ActivationTokenExpiredException>();
    }

    // ─── Already used token ────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenTokenAlreadyUsed_ThrowsActivationTokenAlreadyUsedException()
    {
        // Arrange
        var (tokenEntity, rawToken) = ActivationToken.CreateNew(1, TimeSpan.FromHours(24));

        // Mark the token as used before the handler sees it
        tokenEntity.MarkAsUsed();

        _activationTokenRepository
            .FindByHashAsync(ActivationToken.ComputeHash(rawToken))
            .Returns(tokenEntity);

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new ActivateAccountCommand(rawToken));

        // Assert
        await act.Should().ThrowAsync<ActivationTokenAlreadyUsedException>();
    }

    // ─── Account not found (defensive) ────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenAccountNotFoundForToken_ThrowsAccountNotFoundException()
    {
        // Arrange
        var (tokenEntity, rawToken) = ActivationToken.CreateNew(99, TimeSpan.FromHours(24));

        _activationTokenRepository
            .FindByHashAsync(ActivationToken.ComputeHash(rawToken))
            .Returns(tokenEntity);
        _accountRepository.FindByIdAsync(99).Returns((Account?)null);

        var sut = CreateSut();

        // Act
        Func<Task> act = () => sut.Handle(new ActivateAccountCommand(rawToken));

        // Assert
        await act.Should().ThrowAsync<global::FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions.AccountNotFoundException>();
    }
}
