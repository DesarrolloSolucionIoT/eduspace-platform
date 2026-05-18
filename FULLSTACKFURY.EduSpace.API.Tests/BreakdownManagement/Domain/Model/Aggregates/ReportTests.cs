using FluentAssertions;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.ValueObjects;
using FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.BreakdownManagement;
using Xunit;

namespace FULLSTACKFURY.EduSpace.API.Tests.BreakdownManagement.Domain.Model.Aggregates;

public class ReportTests
{
    // ── Construction ─────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_ValidFields_SetsPropertiesCorrectly()
    {
        // Arrange
        var createdAt = new DateTime(2026, 1, 10, 8, 0, 0, DateTimeKind.Utc);

        // Act
        var report = new ReportBuilder()
            .WithKindOfReport("Water leak")
            .WithDescription("Pipe broken in lab.")
            .WithResourceId(3)
            .WithCreatedAt(createdAt)
            .Build();

        // Assert
        report.KindOfReport.Should().Be("Water leak");
        report.Description.Should().Be("Pipe broken in lab.");
        report.ResourceId.Id.Should().Be(3);
        report.CreatedAt.Should().Be(createdAt);
        report.Status.Should().Be(ReportStatus.EnEspera);
    }

    [Fact]
    public void Constructor_WithCreateReportCommand_SetsStatusToEnEspera()
    {
        // Arrange
        var command = new CreateReportCommand("Fire alarm", "Alarm triggered.", 2, DateTime.UtcNow);

        // Act
        var report = new Report(command);

        // Assert
        report.Status.Should().Be(ReportStatus.EnEspera);
    }

    [Fact]
    public void Constructor_WithExplicitStatus_UsesProvidedStatus()
    {
        // Arrange & Act
        var report = new ReportBuilder()
            .InProgress()
            .Build();

        // Assert
        report.Status.Should().Be(ReportStatus.EnProceso);
    }

    [Fact]
    public void Constructor_WithNullStatus_DefaultsToEnEspera()
    {
        // Arrange & Act
        var report = new ReportBuilder()
            .Build();

        // Assert
        report.Status.Should().Be(ReportStatus.EnEspera);
    }

    // ── MarkAsInProgress — valid transition ──────────────────────────────────

    [Fact]
    public void MarkAsInProgress_WhenStatusIsEnEspera_TransitionsToEnProceso()
    {
        // Arrange
        var report = new ReportBuilder().Build(); // starts at EnEspera

        // Act
        report.MarkAsInProgress();

        // Assert
        report.Status.Should().Be(ReportStatus.EnProceso);
    }

    [Fact]
    public void MarkAsInProgress_WhenStatusIsEnEspera_ReturnsSameReportInstance()
    {
        // Arrange
        var report = new ReportBuilder().Build();

        // Act
        var returned = report.MarkAsInProgress();

        // Assert
        returned.Should().BeSameAs(report);
    }

    // ── MarkAsInProgress — invalid transitions ────────────────────────────────

    [Fact]
    public void MarkAsInProgress_WhenAlreadyInProgress_ThrowsInvalidReportTransitionException()
    {
        // Arrange
        var report = new ReportBuilder().InProgress().Build();

        // Act
        Action act = () => report.MarkAsInProgress();

        // Assert
        act.Should().Throw<InvalidReportTransitionException>()
            .WithMessage("*Cannot transition report from 'in progress' to 'in progress'*");
    }

    [Fact]
    public void MarkAsInProgress_WhenCompleted_ThrowsInvalidReportTransitionException()
    {
        // Arrange
        var report = new ReportBuilder().Completed().Build();

        // Act
        Action act = () => report.MarkAsInProgress();

        // Assert
        act.Should().Throw<InvalidReportTransitionException>()
            .WithMessage("*Cannot transition report from 'completed' to 'in progress'*");
    }

    // ── MarkAsCompleted — valid transition ────────────────────────────────────

    [Fact]
    public void MarkAsCompleted_WhenStatusIsEnProceso_TransitionsToCompletado()
    {
        // Arrange
        var report = new ReportBuilder().InProgress().Build();

        // Act
        report.MarkAsCompleted();

        // Assert
        report.Status.Should().Be(ReportStatus.Completado);
    }

    [Fact]
    public void MarkAsCompleted_WhenStatusIsEnProceso_ReturnsSameReportInstance()
    {
        // Arrange
        var report = new ReportBuilder().InProgress().Build();

        // Act
        var returned = report.MarkAsCompleted();

        // Assert
        returned.Should().BeSameAs(report);
    }

    // ── MarkAsCompleted — invalid transitions ─────────────────────────────────

    [Fact]
    public void MarkAsCompleted_WhenStatusIsEnEspera_ThrowsInvalidReportTransitionException()
    {
        // Arrange
        var report = new ReportBuilder().Build(); // EnEspera

        // Act
        Action act = () => report.MarkAsCompleted();

        // Assert
        act.Should().Throw<InvalidReportTransitionException>()
            .WithMessage("*Cannot transition report from 'pending' to 'completed'*");
    }

    [Fact]
    public void MarkAsCompleted_WhenAlreadyCompleted_ThrowsInvalidReportTransitionException()
    {
        // Arrange
        var report = new ReportBuilder().Completed().Build();

        // Act
        Action act = () => report.MarkAsCompleted();

        // Assert
        act.Should().Throw<InvalidReportTransitionException>()
            .WithMessage("*Cannot transition report from 'completed' to 'completed'*");
    }

    // ── Full valid state machine path ─────────────────────────────────────────

    [Fact]
    public void FullTransitionPath_EnEspera_ToEnProceso_ToCompletado_Succeeds()
    {
        // Arrange
        var report = new ReportBuilder().Build();

        // Act
        report.MarkAsInProgress();
        report.MarkAsCompleted();

        // Assert
        report.Status.Should().Be(ReportStatus.Completado);
    }

    // ── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public void Update_ValidCommand_UpdatesKindOfReportAndDescription()
    {
        // Arrange
        var report = new ReportBuilder()
            .WithKindOfReport("Old kind")
            .WithDescription("Old description")
            .Build();
        var command = new UpdateReportCommand(1, "New kind", "New description");

        // Act
        report.Update(command);

        // Assert
        report.KindOfReport.Should().Be("New kind");
        report.Description.Should().Be("New description");
    }

    [Fact]
    public void Update_ValidCommand_DoesNotChangeStatus()
    {
        // Arrange
        var report = new ReportBuilder().InProgress().Build();
        var command = new UpdateReportCommand(1, "Updated kind", "Updated description");

        // Act
        report.Update(command);

        // Assert
        report.Status.Should().Be(ReportStatus.EnProceso);
    }

    [Fact]
    public void Update_ValidCommand_ReturnsSameReportInstance()
    {
        // Arrange
        var report = new ReportBuilder().Build();
        var command = new UpdateReportCommand(1, "Kind", "Desc");

        // Act
        var returned = report.Update(command);

        // Assert
        returned.Should().BeSameAs(report);
    }
}
