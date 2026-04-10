using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Transform;

public static class CreateAdminProfileCommandFromResourceAssembler
{
    public static CreateAdministratorProfileCommand ToCommandFromResource(CreateAdminProfileResource resource)
    {
        return new CreateAdministratorProfileCommand(resource.FirstName, resource.LastName
            , resource.Email, resource.Dni, resource.Address, resource.Phone
            , resource.Username, resource.Password);
    }
}