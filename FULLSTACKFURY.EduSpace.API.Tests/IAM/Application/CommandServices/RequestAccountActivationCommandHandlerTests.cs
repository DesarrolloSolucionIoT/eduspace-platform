using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.CommandServices;
using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.IAM.Application.CommandServices;

public class RequestAccountActivationCommandHandlerTests
{
    private readonly IActivationTokenRepository _activationTokenRepository = Substitute.For<IActivationTokenRepository>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILogger<RequestAccountActivationCommandHandler> _logger =
        Substitute.For<ILogger<RequestAccountActivationCommandHandler>>();

    private RequestAccountActivationCommandHandler CreateSut() =>
        new(_activationTokenRepository, _emailService, _unitOfWork, _logger);

    // ─── Happy path ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenValidCommand_PersistsTokenBeforeSendingEmail()
    {
        // Arrange
        var command = new RequestAccountActivationCommand(1, "admin@example.com", "John Smith");
        var callOrder = new List<string>();

        _unitOfWork.CompleteAsync().Returns(callInfo =>
        {
            callOrder.Add("uow");
            return Task.CompletedTask;
        });

        _emailService.SendActivationEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(callInfo =>
            {
                callOrder.Add("email");
                return Task.CompletedTask;
            });

        var sut = CreateSut();

        // Act
        await sut.Handle(command);

        // Assert — UoW must complete before email is sent
        callOrder.Should().ContainInOrder("uow", "email");
    }

    [Fact]
    public async Task Handle_WhenValidCommand_AddsActivationTokenToRepository()
    {
        // Arrange
        var command = new RequestAccountActivationCommand(42, "user@example.com", "Jane Doe");

        var sut = CreateSut();

        // Act
        await sut.Handle(command);

        // Assert
        await _activationTokenRepository.Received(1)
            .AddAsync(Arg.Is<global::FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates.ActivationToken>(
                t => t.AccountId == 42));
    }

    [Fact]
    public async Task Handle_WhenValidCommand_CallsCompleteAsyncOnce()
    {
        // Arrange
        var command = new RequestAccountActivationCommand(1, "admin@example.com", "John Smith");

        var sut = CreateSut();

        // Act
        await sut.Handle(command);

        // Assert
        await _unitOfWork.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task Handle_WhenValidCommand_SendsActivationEmail()
    {
        // Arrange
        var command = new RequestAccountActivationCommand(1, "admin@example.com", "John Smith");

        var sut = CreateSut();

        // Act
        await sut.Handle(command);

        // Assert
        await _emailService.Received(1)
            .SendActivationEmailAsync("admin@example.com", "John Smith", Arg.Any<string>());
    }

    // ─── Email failure — log + swallow (Design Decision 4) ────────────────────────

    [Fact]
    public async Task Handle_WhenEmailThrows_TokenIsAlreadyPersisted()
    {
        // Arrange
        var command = new RequestAccountActivationCommand(1, "admin@example.com", "John Smith");

        // Simulate Resend failure
        _emailService.SendActivationEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Throws(new InvalidOperationException("Resend failed"));

        var sut = CreateSut();

        // Act — must not rethrow
        await sut.Handle(command);

        // Assert — UoW was still called
        await _unitOfWork.Received(1).CompleteAsync();

        // Assert — token was still persisted
        await _activationTokenRepository.Received(1).AddAsync(Arg.Any<global::FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates.ActivationToken>());
    }

    [Fact]
    public async Task Handle_WhenEmailThrows_DoesNotRethrow()
    {
        // Arrange
        var command = new RequestAccountActivationCommand(1, "admin@example.com", "John Smith");

        _emailService.SendActivationEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Throws(new InvalidOperationException("Network error"));

        var sut = CreateSut();

        // Act & Assert — exception must not propagate
        Func<Task> act = () => sut.Handle(command);
        await act.Should().NotThrowAsync();
    }
}
