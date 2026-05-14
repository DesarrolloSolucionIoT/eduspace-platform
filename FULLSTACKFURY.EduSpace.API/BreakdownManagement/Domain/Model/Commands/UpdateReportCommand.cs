namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Commands;

/// <summary>
/// Updates editable content fields on a Report (KindOfReport, Description).
/// Status transitions are handled by dedicated commands: MarkAsInProgressCommand / MarkAsCompletedCommand,
/// which are dispatched by the command service based on the target status provided by the REST layer.
/// </summary>
public record UpdateReportCommand(int Id, string KindOfReport, string Description, string? TargetStatus = null);
