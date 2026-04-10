namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Interface.REST.Resources;

public record ReportResource(
    int Id,
    string KindOfReport,
    string Description,
    int ResourceId,
    DateTime CreatedAt,
    string Status);