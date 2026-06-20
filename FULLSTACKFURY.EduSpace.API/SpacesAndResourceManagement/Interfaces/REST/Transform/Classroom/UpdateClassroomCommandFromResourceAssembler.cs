using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Classroom;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Classroom;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.Classroom;

public static class UpdateClassroomCommandFromResourceAssembler
{
    // Single-zone deployment: the UI doesn't expose zoneId. Without this default an update
    // would send null and wipe the classroom's zone, breaking its link to the IoT monitoring.
    private const string DefaultZoneId = "aula-101";

    public static UpdateClassroomCommand ToCommandFromResource(int id, UpdateClassroomResource resource)
    {
        var zoneId = string.IsNullOrWhiteSpace(resource.ZoneId) ? DefaultZoneId : resource.ZoneId;
        return new UpdateClassroomCommand(
            resource.Id,
            resource.Name,
            resource.Description,
            resource.TeacherId,
            zoneId
        );
    }
}