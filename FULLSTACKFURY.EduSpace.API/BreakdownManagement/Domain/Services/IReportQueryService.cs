using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Queries;

namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Services;

public interface IReportQueryService
{
    Task<IEnumerable<Report>> Handle(GetAllReportsQuery query);
    Task<IEnumerable<Report>> Handle(GetAllReportsByResourceIdQuery query);
    Task<Report?> Handle(GetReportByIdQuery query);
}