using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Aggregates;

public class Report
{
    /// <summary>
    /// Parameterless constructor required by EF Core for rehydration.
    /// Do NOT use directly in application code.
    /// </summary>
    public Report()
    {
        KindOfReport = string.Empty;
        Description = string.Empty;
        Status = ReportStatus.EnEspera;
        ResourceId = new ResourceId(1); // Safe EF placeholder — overwritten by EF property mapping.
    }

    public Report(string kindOfReport, string description, int resourceId, DateTime createdAt,
        ReportStatus? status = null)
    {
        KindOfReport = kindOfReport;
        Description = description;
        ResourceId = new ResourceId(resourceId);
        CreatedAt = createdAt;
        Status = status ?? ReportStatus.EnEspera;
    }

    public Report(CreateReportCommand command)
    {
        KindOfReport = command.KindOfReport;
        Description = command.Description;
        ResourceId = new ResourceId(command.ResourceId);
        CreatedAt = command.CreatedAt;
        Status = ReportStatus.EnEspera;
    }

    public int Id { get; init; }
    public string KindOfReport { get; private set; }
    public string Description { get; private set; }
    public ResourceId ResourceId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Status is read-only externally. Transitions must go through domain methods:
    /// <see cref="MarkAsInProgress"/>, <see cref="MarkAsCompleted"/>.
    /// </summary>
    public ReportStatus Status { get; private set; }

    // ── State machine ──────────────────────────────────────────────────────────

    /// <summary>
    /// Transitions status from <c>EnEspera</c> (pending) → <c>EnProceso</c> (in progress).
    /// Throws <see cref="InvalidReportTransitionException"/> if the current status is not pending.
    /// </summary>
    public Report MarkAsInProgress()
    {
        if (Status != ReportStatus.EnEspera)
            throw new InvalidReportTransitionException(Status.Value, ReportStatus.EnProceso.Value);

        Status = ReportStatus.EnProceso;
        return this;
    }

    /// <summary>
    /// Transitions status from <c>EnProceso</c> (in progress) → <c>Completado</c> (completed).
    /// Throws <see cref="InvalidReportTransitionException"/> if the current status is not in progress.
    /// </summary>
    public Report MarkAsCompleted()
    {
        if (Status != ReportStatus.EnProceso)
            throw new InvalidReportTransitionException(Status.Value, ReportStatus.Completado.Value);

        Status = ReportStatus.Completado;
        return this;
    }

    // ── General update ────────────────────────────────────────────────────────

    /// <summary>
    /// Updates editable fields (KindOfReport, Description).
    /// Does NOT change Status — use <see cref="MarkAsInProgress"/> / <see cref="MarkAsCompleted"/>.
    /// </summary>
    public Report Update(UpdateReportCommand command)
    {
        KindOfReport = command.KindOfReport;
        Description = command.Description;
        return this;
    }
}
