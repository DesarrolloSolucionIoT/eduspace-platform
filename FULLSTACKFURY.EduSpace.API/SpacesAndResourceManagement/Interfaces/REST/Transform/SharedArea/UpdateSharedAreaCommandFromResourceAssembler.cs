using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.SharedArea;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.SharedArea;

public static class UpdateSharedAreaCommandFromResourceAssembler
{
    public static UpdateSharedAreaCommand ToCommandFromResource(int Id, UpdateSharedAreaResource resource)
    {
        return new UpdateSharedAreaCommand(
            Id,
            resource.Name,
            resource.Capacity,
            resource.Description
        );
    }
}