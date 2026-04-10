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
    public static CreateClassroomCommand ToCommandFromResource(int teacherId, CreateClassroomResource resource)
    {
        return new CreateClassroomCommand(resource.Name, resource.Description, teacherId);
    }
}