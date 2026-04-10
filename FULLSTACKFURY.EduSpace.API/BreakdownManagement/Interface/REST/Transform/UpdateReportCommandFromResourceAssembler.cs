using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.BreakdownManagement.Interface.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Interface.REST.Transform;

public static class UpdateReportCommandFromResourceAssembler
{
    public static UpdateReportCommand ToCommandFromResource(int id, UpdateReportResource resource)
    {
        return new UpdateReportCommand(id, resource.KindOfReport, resource.Description, resource.Status);
    }
}