using System.ComponentModel.DataAnnotations;

namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Interface.REST.Resources;

/// <summary>
/// Resource for updating a report's content and optionally its status.
/// </summary>
/// <param name="KindOfReport">Report type (required).</param>
/// <param name="Description">Report description (required).</param>
/// <param name="Status">
/// Target status for transition. Allowed values: "in progress", "completed".
/// If null or omitted, no status transition is applied.
/// </param>
public record UpdateReportResource(
    [Required] string KindOfReport,
    [Required] string Description,
    string? Status = null);
