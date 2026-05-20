using System.ComponentModel.DataAnnotations;

namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Interfaces.REST.Resources;

public record CreateReportResource(
    [Required] string KindOfReport,
    [Required] string Description,
    [Range(1, int.MaxValue)] int ResourceId,
    DateTime CreatedAt);
