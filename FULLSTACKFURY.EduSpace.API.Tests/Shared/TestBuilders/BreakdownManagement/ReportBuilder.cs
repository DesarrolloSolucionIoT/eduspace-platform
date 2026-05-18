using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.Tests.Shared.TestBuilders.BreakdownManagement;

/// <summary>
/// Fluent builder for <see cref="Report"/> test instances.
/// Instantiates Report directly — no mocks for same-context aggregates.
/// </summary>
public class ReportBuilder
{
    private string _kindOfReport = "Electrical failure";
    private string _description = "Short circuit in room 3A.";
    private int _resourceId = 1;
    private DateTime _createdAt = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc);
    private ReportStatus? _status = null;

    public ReportBuilder WithKindOfReport(string kindOfReport)
    {
        _kindOfReport = kindOfReport;
        return this;
    }

    public ReportBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ReportBuilder WithResourceId(int resourceId)
    {
        _resourceId = resourceId;
        return this;
    }

    public ReportBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public ReportBuilder WithStatus(ReportStatus status)
    {
        _status = status;
        return this;
    }

    public ReportBuilder InProgress()
    {
        _status = ReportStatus.EnProceso;
        return this;
    }

    public ReportBuilder Completed()
    {
        _status = ReportStatus.Completado;
        return this;
    }

    public Report Build() =>
        new Report(_kindOfReport, _description, _resourceId, _createdAt, _status);
}
