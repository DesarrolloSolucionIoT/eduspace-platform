using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Services;

namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Application.Internal.QueryServices;

public class ReportQueryService : IReportQueryService
{
    private readonly IReportRepository _reportRepository;

    public ReportQueryService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public Task<IEnumerable<Report>> Handle(GetAllReportsQuery query)
    {
        return _reportRepository.ListAsync();
    }

    public Task<IEnumerable<Report>> Handle(GetAllReportsByResourceIdQuery query)
    {
        return _reportRepository.FindAllByResourceIdAsync(query.ResourceId);
    }

    public async Task<Report?> Handle(GetReportByIdQuery query)
    {
        return await _reportRepository.FindByIdAsync(query.Id);
    }
}