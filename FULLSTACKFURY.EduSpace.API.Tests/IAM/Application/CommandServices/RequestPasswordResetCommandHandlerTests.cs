using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.CommandServices;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.IAM.Application.CommandServices;

public class RequestPasswordResetCommandHandlerTests
{
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository =
        Substitute.For<IPasswordResetTokenRepository>();
    private readonly ITeacherProfileRepository _teacherProfileRepository =
        Substitute.For<ITeacherProfileRepository>();
    private readonly IAdminProfileRepository _adminProfileRepository =
        Substitute.For<IAdminProfileRepository>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILogger<RequestPasswordResetCommandHandler> _logger =
        Substitute.For<ILogger<RequestPasswordResetCommandHandler>>();

    private RequestPasswordResetCommandHandler CreateSut() =>
        new(_passwordResetTokenRepository, _teacherProfileRepository, _adminProfileRepository,
            _emailService, _unitOfWork, _logger);

    // ─── Unknown email — anti-enumeration (silent no-op) ──────────────────────────

    [Fact]
    public async Task Handle_WhenEmailUnknown_DoesNotAddToken()
    {
        // Arrange
        _teacherProfileRepository.FindAccountIdByEmailAsync(Arg.Any<string>()).Returns((int?)null);
        _adminProfileRepository.FindAccountIdByEmailAsync(Arg.Any<string>()).Returns((int?)null);

        var sut = CreateSut();

        // Act
        await sut.Handle(new RequestPasswordResetCommand("unknown@example.com"));

        // Assert
        await _passwordResetTokenRepository.DidNotReceive().AddAsync(Arg.Any<PasswordResetToken>());
    }

    [Fact]
    public async Task Handle_WhenEmailUnknown_DoesNotSendEmail()
    {
        // Arrange
        _teacherProfileRepository.FindAccountIdByEmailAsync(Arg.Any<string>()).Returns((int?)null);
        _adminProfileRepository.FindAccountIdByEmailAsync(Arg.Any<string>()).Returns((int?)null);

        var sut = CreateSut();

        // Act
        await sut.Handle(new RequestPasswordResetCommand("unknown@example.com"));

        // Assert
        await _emailService.DidNotReceive()
            .SendPasswordResetEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_WhenEmailUnknown_DoesNotThrow()
    {
        // Arrange
        _teacherProfileRepository.FindAccountIdByEmailAsync(Arg.Any<string>()).Returns((int?)null);
        _adminProfileRepository.FindAccountIdByEmailAsync(Arg.Any<string>()).Returns((int?)null);

        var sut = CreateSut();

        // Act & Assert
        Func<Task> act = () => sut.Handle(new RequestPasswordResetCommand("unknown@example.com"));
        await act.Should().NotThrowAsync();
    }

    // ─── Known email (teacher) ─────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenTeacherEmailFound_AddsTokenForResolvedAccount()
    {
        // Arrange
        _teacherProfileRepository.FindAccountIdByEmailAsync("teacher@example.com").Returns(7);
        _adminProfileRepository.FindAccountIdByEmailAsync(Arg.Any<string>()).Returns((int?)null);

        var sut = CreateSut();

        // Act
        await sut.Handle(new RequestPasswordResetCommand("teacher@example.com"));

        // Assert
        await _passwordResetTokenRepository.Received(1)
            .AddAsync(Arg.Is<PasswordResetToken>(t => t.AccountId == 7));
    }

    [Fact]
    public async Task Handle_WhenTeacherEmailFound_SendsPasswordResetEmailOnce()
    {
        // Arrange
        _teacherProfileRepository.FindAccountIdByEmailAsync("teacher@example.com").Returns(7);
        _adminProfileRepository.FindAccountIdByEmailAsync(Arg.Any<string>()).Returns((int?)null);

        var sut = CreateSut();

        // Act
        await sut.Handle(new RequestPasswordResetCommand("teacher@example.com"));

        // Assert
        await _emailService.Received(1)
            .SendPasswordResetEmailAsync("teacher@example.com", Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_WhenTeacherEmailFound_PersistsTokenBeforeSendingEmail()
    {
        // Arrange
        _teacherProfileRepository.FindAccountIdByEmailAsync("teacher@example.com").Returns(7);
        _adminProfileRepository.FindAccountIdByEmailAsync(Arg.Any<string>()).Returns((int?)null);

        var callOrder = new List<string>();
        _unitOfWork.CompleteAsync().Returns(_ =>
        {
            callOrder.Add("uow");
            return Task.CompletedTask;
        });
        _emailService.SendPasswordResetEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(_ =>
            {
                callOrder.Add("email");
                return Task.CompletedTask;
            });

        var sut = CreateSut();

        // Act
        await sut.Handle(new RequestPasswordResetCommand("teacher@example.com"));

        // Assert
        callOrder.Should().ContainInOrder("uow", "email");
    }

    // ─── Known email (admin fallback) ──────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenAdminEmailFound_AddsTokenAndSendsEmail()
    {
        // Arrange
        _teacherProfileRepository.FindAccountIdByEmailAsync("admin@example.com").Returns((int?)null);
        _adminProfileRepository.FindAccountIdByEmailAsync("admin@example.com").Returns(13);

        var sut = CreateSut();

        // Act
        await sut.Handle(new RequestPasswordResetCommand("admin@example.com"));

        // Assert
        await _passwordResetTokenRepository.Received(1)
            .AddAsync(Arg.Is<PasswordResetToken>(t => t.AccountId == 13));
        await _emailService.Received(1)
            .SendPasswordResetEmailAsync("admin@example.com", Arg.Any<string>(), Arg.Any<string>());
    }
}
