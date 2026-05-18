using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.QueryServices;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.IAM;
using NSubstitute;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.IAM.Application.QueryServices;

public class AccountQueryServiceTests
{
    private readonly IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();

    private AccountQueryService CreateSut() => new(_accountRepository);

    // ─── GetAccountByIdQuery ─────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_GetAccountByIdQuery_WhenAccountExists_ReturnsAccount()
    {
        // Arrange
        var account = new AccountBuilder().AsAdmin().Build();
        _accountRepository.FindByIdAsync(1).Returns(account);

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new GetAccountByIdQuery(1));

        // Assert
        result.Should().BeSameAs(account);
    }

    [Fact]
    public async Task Handle_GetAccountByIdQuery_WhenAccountDoesNotExist_ReturnsNull()
    {
        // Arrange
        _accountRepository.FindByIdAsync(99).Returns((Account?)null);

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new GetAccountByIdQuery(99));

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAccountByIdQuery_WhenCalled_DelegatesToRepository()
    {
        // Arrange
        _accountRepository.FindByIdAsync(5).Returns((Account?)null);

        var sut = CreateSut();

        // Act
        await sut.Handle(new GetAccountByIdQuery(5));

        // Assert
        await _accountRepository.Received(1).FindByIdAsync(5);
    }

    // ─── GetAccountByUsernameQuery ───────────────────────────────────────────────

    [Fact]
    public async Task Handle_GetAccountByUsernameQuery_WhenAccountExists_ReturnsAccount()
    {
        // Arrange
        const string username = "admin@example.com";
        var account = new AccountBuilder().WithUsername(username).AsAdmin().Build();
        _accountRepository.FindByUsername(username).Returns(account);

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new GetAccountByUsernameQuery(username));

        // Assert
        result.Should().BeSameAs(account);
    }

    [Fact]
    public async Task Handle_GetAccountByUsernameQuery_WhenAccountDoesNotExist_ReturnsNull()
    {
        // Arrange
        const string username = "unknown@example.com";
        _accountRepository.FindByUsername(username).Returns((Account?)null);

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new GetAccountByUsernameQuery(username));

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAccountByUsernameQuery_WhenCalled_DelegatesToRepository()
    {
        // Arrange
        const string username = "teacher@example.com";
        _accountRepository.FindByUsername(username).Returns((Account?)null);

        var sut = CreateSut();

        // Act
        await sut.Handle(new GetAccountByUsernameQuery(username));

        // Assert
        await _accountRepository.Received(1).FindByUsername(username);
    }
}
