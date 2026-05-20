using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Interfaces.REST.Transform;

public static class ReportResourceFromEntityAssembler
{
    public static ReportResource ToResourceFromEntity(Report entity)
    {
        return new ReportResource(
            entity.Id,
            entity.KindOfReport,
            entity.Description,
            entity.ResourceId.Id,
            entity.CreatedAt,
            entity.Status.Value
        );
    }
}