using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Transform;

public static class UpdateAdminProfileCommandFromResourceAssembler
{
    public static UpdateAdminProfileCommand ToCommandFromResource(int id, UpdateAdminProfileResource resource)
    {
        return new UpdateAdminProfileCommand(id, resource.FirstName, resource.LastName, resource.Email, resource.Dni,
            resource.Address, resource.Phone);
    }
}