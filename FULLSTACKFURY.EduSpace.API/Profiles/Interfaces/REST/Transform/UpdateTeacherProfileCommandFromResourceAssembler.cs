using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Transform;

public static class UpdateTeacherProfileCommandFromResourceAssembler
{
    public static UpdateTeacherProfileCommand ToCommandFromResource(int id, UpdateTeacherProfileResource resource)
    {
        return new UpdateTeacherProfileCommand(id, resource.FirstName, resource.LastName, resource.Email, resource.Dni,
            resource.Address, resource.Phone);
    }
}