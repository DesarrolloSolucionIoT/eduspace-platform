using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.SharedArea;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.SharedArea;

/// <summary>
///     Assembler class to transform CreateSharedAreaResource to CreateSharedAreaCommand
/// </summary>
public class CreateSharedAreaCommandFromResourceAssembler
{
    /// <summary>
    ///     Transform CreateSharedAreaResource to CreateSharedAreaCommand
    /// </summary>
    /// <param name="resource">
    ///     The <see cref="CreateSharedAreaResource" /> resource to transform
    /// </param>
    /// <returns>
    ///     The resulting <see cref="CreateSharedAreaCommand" /> command with the values from the resource
    /// </returns>
    // Single-zone deployment: the UI doesn't expose zoneId, so when it isn't provided we
    // default it to the one zone monitored by the edge device. This lets the shared area
    // receive the IoT monitoring data (SensorReading.ZoneId == SharedArea.ZoneId).
    private const string DefaultZoneId = "aula-101";

    public static CreateSharedAreaCommand ToCommandFromResource(CreateSharedAreaResource resource)
    {
        var zoneId = string.IsNullOrWhiteSpace(resource.ZoneId) ? DefaultZoneId : resource.ZoneId;
        return new CreateSharedAreaCommand(resource.Name, resource.Capacity, resource.Description, zoneId);
    }
}