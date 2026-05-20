using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Interfaces.REST.Transform;

public static class CreateReportCommandFromResourceAssembler
{
    public static CreateReportCommand ToCommandFromResource(CreateReportResource resource)
    {
        return new CreateReportCommand(
            resource.KindOfReport,
            resource.Description,
            resource.ResourceId,
            resource.CreatedAt
        );
    }
}