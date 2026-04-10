using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Transform;

public static class AccountResourceFromEntityAssembler
{
    public static AccountResource ToResourceFromEntity(Account entity)
    {
        return new AccountResource(entity.Id, entity.Username);
    }
}