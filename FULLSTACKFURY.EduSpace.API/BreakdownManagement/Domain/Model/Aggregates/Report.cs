using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Aggregates;

public record Report
{
    public Report()
    {
        KindOfReport = string.Empty;
        Description = string.Empty;
        Status = ReportStatus.EnProceso;
        ResourceId = default!;
    }

    public Report(string kindOfReport, string description, int resourceId, DateTime createdAt,
        ReportStatus? status = null)
    {
        KindOfReport = kindOfReport;
        Description = description;
        ResourceId = new ResourceId(resourceId);
        CreatedAt = createdAt;
        Status = status ?? ReportStatus.EnProceso; // Default status
    }

    public Report(CreateReportCommand command)
    {
        KindOfReport = command.KindOfReport;
        Description = command.Description;
        ResourceId = new ResourceId(command.ResourceId);
        CreatedAt = command.CreatedAt;
        Status = ReportStatus.EnProceso; // Default status
    }

    public int Id { get; init; }
    public string KindOfReport { get; private set; }
    public string Description { get; private set; }
    public ResourceId ResourceId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ReportStatus Status { get; private set; }

    public Report Update(UpdateReportCommand command)
    {
        KindOfReport = command.KindOfReport;
        Description = command.Description;
        Status = ReportStatus.FromString(command.Status);
        return this;
    }
}