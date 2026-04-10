using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Classroom;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Classroom;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.Classroom;

public static class UpdateClassroomCommandFromResourceAssembler
{
    public static UpdateClassroomCommand ToCommandFromResource(int id, UpdateClassroomResource resource)
    {
        return new UpdateClassroomCommand(
            resource.Id,
            resource.Name,
            resource.Description,
            resource.TeacherId
        );
    }
}