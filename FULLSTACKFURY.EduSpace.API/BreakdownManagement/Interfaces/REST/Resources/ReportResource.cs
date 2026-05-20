namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Interfaces.REST.Resources;

/// <summary>
/// REST response representation of a Report.
/// </summary>
/// <remarks>
/// VO SERIALIZATION NOTE: Project convention (see CLAUDE.md) is to serialize VOs as nested objects,
/// e.g. <c>ResourceId: { id: number }</c> and <c>Status: { value: string }</c>.
/// Currently both are emitted as flat primitives to preserve backward compatibility with existing clients.
/// TODO: align with project-wide VO serialization convention once all clients are updated.
/// </remarks>
public record ReportResource(
    int Id,
    string KindOfReport,
    string Description,
    int ResourceId,
    DateTime CreatedAt,
    string Status);
