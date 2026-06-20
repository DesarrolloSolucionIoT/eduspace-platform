using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Classroom;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Classroom;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.Classroom;

/// <summary>
///     Assembler class to transform CreateClassroomResource to CreateClassroomCommand
/// </summary>
public class CreateClassroomCommandFromResourceAssembler
{
    /// <summary>
    ///     Transform CreateClassroomResource to CreateClassroomCommand
    /// </summary>
    /// <param name="resource">
    ///     The <see cref="CreateClassroomResource" /> resource to transform
    /// </param>
    /// <returns>
    ///     The resulting <see cref="CreateClassroomCommand" /> command with the values from the resource
    /// </returns>
    // Single-zone deployment: the UI doesn't expose zoneId, so when it isn't provided we
    // default it to the one classroom zone monitored by the edge device. This lets the
    // classroom receive the IoT monitoring data (SensorReading.ZoneId == Classroom.ZoneId).
    private const string DefaultZoneId = "aula-101";

    public static CreateClassroomCommand ToCommandFromResource(int teacherId, CreateClassroomResource resource)
    {
        var zoneId = string.IsNullOrWhiteSpace(resource.ZoneId) ? DefaultZoneId : resource.ZoneId;
        return new CreateClassroomCommand(resource.Name, resource.Description, teacherId, zoneId);
    }
}