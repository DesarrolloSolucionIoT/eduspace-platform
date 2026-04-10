namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Commands;

public record UpdateReportCommand(int Id, string KindOfReport, string Description, string Status);