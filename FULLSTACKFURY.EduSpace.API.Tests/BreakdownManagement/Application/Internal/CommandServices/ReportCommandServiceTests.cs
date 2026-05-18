using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Application.Internal.CommandServices;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Application.Internal.OutboundServices;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.ValueObjects;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.BreakdownManagement;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.BreakdownManagement.Application.Internal.CommandServices;

public class ReportCommandServiceTests
{
    private readonly IReportRepository _reportRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExternalResourceService _externalResourceService;
    private readonly ILogger<ReportCommandService> _logger;
    private readonly ReportCommandService _sut;

    public ReportCommandServiceTests()
    {
        _reportRepository = Substitute.For<IReportRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _externalResourceService = Substitute.For<IExternalResourceService>();
        _logger = Substitute.For<ILogger<ReportCommandService>>();

        _sut = new ReportCommandService(
            _reportRepository,
            _unitOfWork,
            _externalResourceService,
            _logger);
    }

    // ── Handle(CreateReportCommand) ───────────────────────────────────────────

    [Fact]
    public async Task Handle_CreateReportCommand_ResourceExists_ReturnsCreatedReport()
    {
        // Arrange
        var command = new CreateReportCommand("Electrical failure", "Short circuit.", 1, DateTime.UtcNow);
        _externalResourceService.ValidateResourceExistsAsync(command.ResourceId).Returns(Task.FromResult(true));
        _reportRepository.AddAsync(Arg.Any<Report>()).Returns(Task.CompletedTask);
        _unitOfWork.CompleteAsync().Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result.Should().NotBeNull();
        result!.KindOfReport.Should().Be("Electrical failure");
        result.Description.Should().Be("Short circuit.");
        result.ResourceId.Id.Should().Be(1);
        result.Status.Should().Be(ReportStatus.EnEspera);
    }

    [Fact]
    public async Task Handle_CreateReportCommand_ResourceExists_CallsRepositoryAddAsync()
    {
        // Arrange
        var command = new CreateReportCommand("Leak", "Water leaking.", 2, DateTime.UtcNow);
        _externalResourceService.ValidateResourceExistsAsync(command.ResourceId).Returns(Task.FromResult(true));
        _reportRepository.AddAsync(Arg.Any<Report>()).Returns(Task.CompletedTask);
        _unitOfWork.CompleteAsync().Returns(Task.CompletedTask);

        // Act
        await _sut.Handle(command);

        // Assert
        await _reportRepository.Received(1).AddAsync(Arg.Any<Report>());
    }

    [Fact]
    public async Task Handle_CreateReportCommand_ResourceExists_CallsUnitOfWorkCompleteAsync()
    {
        // Arrange
        var command = new CreateReportCommand("Leak", "Water leaking.", 2, DateTime.UtcNow);
        _externalResourceService.ValidateResourceExistsAsync(command.ResourceId).Returns(Task.FromResult(true));
        _reportRepository.AddAsync(Arg.Any<Report>()).Returns(Task.CompletedTask);
        _unitOfWork.CompleteAsync().Returns(Task.CompletedTask);

        // Act
        await _sut.Handle(command);

        // Assert
        await _unitOfWork.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task Handle_CreateReportCommand_ResourceDoesNotExist_ThrowsResourceNotFoundForReportException()
    {
        // Arrange
        var command = new CreateReportCommand("Fire", "Smoke detected.", 99, DateTime.UtcNow);
        _externalResourceService.ValidateResourceExistsAsync(command.ResourceId).Returns(Task.FromResult(false));

        // Act
        Func<Task> act = () => _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundForReportException>()
            .WithMessage("*Resource with ID 99*");
    }

    [Fact]
    public async Task Handle_CreateReportCommand_ResourceDoesNotExist_DoesNotCallRepository()
    {
        // Arrange
        var command = new CreateReportCommand("Fire", "Smoke detected.", 99, DateTime.UtcNow);
        _externalResourceService.ValidateResourceExistsAsync(command.ResourceId).Returns(Task.FromResult(false));

        // Act
        try { await _sut.Handle(command); } catch { /* expected */ }

        // Assert
        await _reportRepository.DidNotReceive().AddAsync(Arg.Any<Report>());
    }

    // ── Handle(UpdateReportCommand) — no status change ────────────────────────

    [Fact]
    public async Task Handle_UpdateReportCommand_NoTargetStatus_UpdatesContentFieldsOnly()
    {
        // Arrange
        var existing = new ReportBuilder()
            .WithKindOfReport("Old kind")
            .WithDescription("Old description")
            .Build();
        var command = new UpdateReportCommand(1, "New kind", "New description");
        _reportRepository.FindByIdAsync(1).Returns(Task.FromResult<Report?>(existing));
        _unitOfWork.CompleteAsync().Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result!.KindOfReport.Should().Be("New kind");
        result.Description.Should().Be("New description");
        result.Status.Should().Be(ReportStatus.EnEspera);
    }

    [Fact]
    public async Task Handle_UpdateReportCommand_NoTargetStatus_CallsRepositoryUpdate()
    {
        // Arrange
        var existing = new ReportBuilder().Build();
        var command = new UpdateReportCommand(1, "Kind", "Desc");
        _reportRepository.FindByIdAsync(1).Returns(Task.FromResult<Report?>(existing));
        _unitOfWork.CompleteAsync().Returns(Task.CompletedTask);

        // Act
        await _sut.Handle(command);

        // Assert
        _reportRepository.Received(1).Update(existing);
    }

    [Fact]
    public async Task Handle_UpdateReportCommand_NoTargetStatus_CallsUnitOfWork()
    {
        // Arrange
        var existing = new ReportBuilder().Build();
        var command = new UpdateReportCommand(1, "Kind", "Desc");
        _reportRepository.FindByIdAsync(1).Returns(Task.FromResult<Report?>(existing));
        _unitOfWork.CompleteAsync().Returns(Task.CompletedTask);

        // Act
        await _sut.Handle(command);

        // Assert
        await _unitOfWork.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task Handle_UpdateReportCommand_ReportNotFound_ThrowsReportNotFoundException()
    {
        // Arrange
        var command = new UpdateReportCommand(42, "Kind", "Desc");
        _reportRepository.FindByIdAsync(42).Returns(Task.FromResult<Report?>(null));

        // Act
        Func<Task> act = () => _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<ReportNotFoundException>()
            .WithMessage("*Report with ID 42*");
    }

    // ── Handle(UpdateReportCommand) — transition to EnProceso ────────────────

    [Fact]
    public async Task Handle_UpdateReportCommand_TargetStatusInProgress_TransitionsToEnProceso()
    {
        // Arrange
        var existing = new ReportBuilder().Build(); // starts at EnEspera
        var command = new UpdateReportCommand(1, "Kind", "Desc", "in progress");
        _reportRepository.FindByIdAsync(1).Returns(Task.FromResult<Report?>(existing));
        _unitOfWork.CompleteAsync().Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result!.Status.Should().Be(ReportStatus.EnProceso);
    }

    [Fact]
    public async Task Handle_UpdateReportCommand_TargetStatusInProgress_WhenAlreadyInProgress_ThrowsInvalidTransition()
    {
        // Arrange
        var existing = new ReportBuilder().InProgress().Build();
        var command = new UpdateReportCommand(1, "Kind", "Desc", "in progress");
        _reportRepository.FindByIdAsync(1).Returns(Task.FromResult<Report?>(existing));

        // Act
        Func<Task> act = () => _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<InvalidReportTransitionException>();
    }

    [Fact]
    public async Task Handle_UpdateReportCommand_TargetStatusInProgress_WhenCompleted_ThrowsInvalidTransition()
    {
        // Arrange
        var existing = new ReportBuilder().Completed().Build();
        var command = new UpdateReportCommand(1, "Kind", "Desc", "in progress");
        _reportRepository.FindByIdAsync(1).Returns(Task.FromResult<Report?>(existing));

        // Act
        Func<Task> act = () => _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<InvalidReportTransitionException>();
    }

    // ── Handle(UpdateReportCommand) — transition to Completado ───────────────

    [Fact]
    public async Task Handle_UpdateReportCommand_TargetStatusCompleted_TransitionsToCompletado()
    {
        // Arrange
        var existing = new ReportBuilder().InProgress().Build();
        var command = new UpdateReportCommand(1, "Kind", "Desc", "completed");
        _reportRepository.FindByIdAsync(1).Returns(Task.FromResult<Report?>(existing));
        _unitOfWork.CompleteAsync().Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result!.Status.Should().Be(ReportStatus.Completado);
    }

    [Fact]
    public async Task Handle_UpdateReportCommand_TargetStatusCompleted_WhenEnEspera_ThrowsInvalidTransition()
    {
        // Arrange
        var existing = new ReportBuilder().Build(); // EnEspera
        var command = new UpdateReportCommand(1, "Kind", "Desc", "completed");
        _reportRepository.FindByIdAsync(1).Returns(Task.FromResult<Report?>(existing));

        // Act
        Func<Task> act = () => _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<InvalidReportTransitionException>();
    }

    [Fact]
    public async Task Handle_UpdateReportCommand_TargetStatusCompleted_WhenAlreadyCompleted_ThrowsInvalidTransition()
    {
        // Arrange
        var existing = new ReportBuilder().Completed().Build();
        var command = new UpdateReportCommand(1, "Kind", "Desc", "completed");
        _reportRepository.FindByIdAsync(1).Returns(Task.FromResult<Report?>(existing));

        // Act
        Func<Task> act = () => _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<InvalidReportTransitionException>();
    }

    // ── Handle(UpdateReportCommand) — invalid TargetStatus string ────────────

    [Fact]
    public async Task Handle_UpdateReportCommand_TargetStatusPending_ThrowsInvalidReportTransitionException()
    {
        // Arrange — "pending" is a valid ReportStatus but not a valid transition target via Update
        // The service throws InvalidReportTransitionException for the pending case
        var existing = new ReportBuilder().Build();
        var command = new UpdateReportCommand(1, "Kind", "Desc", "pending");
        _reportRepository.FindByIdAsync(1).Returns(Task.FromResult<Report?>(existing));

        // Act
        Func<Task> act = () => _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<InvalidReportTransitionException>();
    }

    [Fact]
    public async Task Handle_UpdateReportCommand_UnknownTargetStatus_ThrowsInvalidReportDataException()
    {
        // Arrange
        var existing = new ReportBuilder().Build();
        var command = new UpdateReportCommand(1, "Kind", "Desc", "bogus-status");
        _reportRepository.FindByIdAsync(1).Returns(Task.FromResult<Report?>(existing));

        // Act
        Func<Task> act = () => _sut.Handle(command);

        // Assert
        // ReportStatus.FromString throws InvalidReportDataException for unknown strings
        await act.Should().ThrowAsync<InvalidReportDataException>();
    }

    // ── Handle(DeleteReportCommand) ───────────────────────────────────────────

    [Fact]
    public async Task Handle_DeleteReportCommand_ExistingReport_RemovesReportAndCompletes()
    {
        // Arrange
        var existing = new ReportBuilder().Build();
        var command = new DeleteReportCommand(1);
        _reportRepository.FindByIdAsync(1).Returns(Task.FromResult<Report?>(existing));
        _unitOfWork.CompleteAsync().Returns(Task.CompletedTask);

        // Act
        await _sut.Handle(command);

        // Assert
        _reportRepository.Received(1).Remove(existing);
    }

    [Fact]
    public async Task Handle_DeleteReportCommand_ExistingReport_CallsUnitOfWork()
    {
        // Arrange
        var existing = new ReportBuilder().Build();
        var command = new DeleteReportCommand(1);
        _reportRepository.FindByIdAsync(1).Returns(Task.FromResult<Report?>(existing));
        _unitOfWork.CompleteAsync().Returns(Task.CompletedTask);

        // Act
        await _sut.Handle(command);

        // Assert
        await _unitOfWork.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task Handle_DeleteReportCommand_ReportNotFound_ThrowsReportNotFoundException()
    {
        // Arrange
        var command = new DeleteReportCommand(99);
        _reportRepository.FindByIdAsync(99).Returns(Task.FromResult<Report?>(null));

        // Act
        Func<Task> act = () => _sut.Handle(command);

        // Assert
        await act.Should().ThrowAsync<ReportNotFoundException>()
            .WithMessage("*Report with ID 99*");
    }

    [Fact]
    public async Task Handle_DeleteReportCommand_ReportNotFound_DoesNotCallRemove()
    {
        // Arrange
        var command = new DeleteReportCommand(99);
        _reportRepository.FindByIdAsync(99).Returns(Task.FromResult<Report?>(null));

        // Act
        try { await _sut.Handle(command); } catch { /* expected */ }

        // Assert
        _reportRepository.DidNotReceive().Remove(Arg.Any<Report>());
    }
}
