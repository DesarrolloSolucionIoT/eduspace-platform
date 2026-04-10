using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Transform;

public static class AdminProfileResourceFromEntityAssembler
{
    public static AdminProfileResource ToResourceFromEntity(AdminProfile entity)
    {
        return new AdminProfileResource(entity.Id, entity.ProfileName.FirstName,
            entity.ProfileName.LastName, entity.ProfilePrivateInformation.ObtainEmail,
            entity.ProfilePrivateInformation.ObtainDni,
            entity.ProfilePrivateInformation.Address,
            entity.ProfilePrivateInformation.Phone);
    }
}