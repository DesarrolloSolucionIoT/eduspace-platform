using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Commands;

namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Services;

public interface IReportCommandService
{
    Task<Report?> Handle(CreateReportCommand command);
    Task<Report?> Handle(UpdateReportCommand command);
    Task Handle(DeleteReportCommand command);
}