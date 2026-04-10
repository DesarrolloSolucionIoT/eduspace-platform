using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Transform;

public static class TeacherProfileResourceFromEntityAssembler
{
    public static TeacherProfileResource ToResourceFromEntity(TeacherProfile entity)
    {
        return new TeacherProfileResource(entity.Id, entity.ProfileName.FirstName,
            entity.ProfileName.LastName, entity.ProfilePrivateInformation.ObtainEmail,
            entity.ProfilePrivateInformation.ObtainDni,
            entity.ProfilePrivateInformation.Address,
            entity.ProfilePrivateInformation.Phone);
    }
}