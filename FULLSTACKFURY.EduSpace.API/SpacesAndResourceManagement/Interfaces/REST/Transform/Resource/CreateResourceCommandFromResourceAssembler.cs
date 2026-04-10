using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Resource;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Resource;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.Resource;

/// <summary>
///     Assembler class to transform CreateClassroomResource to CreateClassroomCommand
/// </summary>
public class CreateResourceCommandFromResourceAssembler
{
    public static CreateResourceCommand ToCommandFromResource(int classroomId, CreateResourceResource resource)
    {
        return new CreateResourceCommand(resource.Name, resource.KindOfResource, classroomId);
    }
}