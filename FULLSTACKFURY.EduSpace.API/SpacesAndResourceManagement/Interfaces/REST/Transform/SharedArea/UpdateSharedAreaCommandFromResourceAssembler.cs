using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.SharedArea;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.SharedArea;

public static class UpdateSharedAreaCommandFromResourceAssembler
{
    // Single-zone deployment: the UI doesn't expose zoneId. Without this default an update
    // would send null and wipe the shared area's zone, breaking its link to the IoT monitoring.
    private const string DefaultZoneId = "aula-101";

    public static UpdateSharedAreaCommand ToCommandFromResource(int Id, UpdateSharedAreaResource resource)
    {
        var zoneId = string.IsNullOrWhiteSpace(resource.ZoneId) ? DefaultZoneId : resource.ZoneId;
        return new UpdateSharedAreaCommand(
            Id,
            resource.Name,
            resource.Capacity,
            resource.Description,
            zoneId
        );
    }
}