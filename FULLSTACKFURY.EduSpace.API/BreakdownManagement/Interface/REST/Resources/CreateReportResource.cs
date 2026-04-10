namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Interface.REST.Resources;

public record CreateReportResource(string KindOfReport, string Description, int ResourceId, DateTime CreatedAt);