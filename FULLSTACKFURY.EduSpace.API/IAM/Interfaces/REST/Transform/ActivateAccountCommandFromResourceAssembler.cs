using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Transform;

public static class ActivateAccountCommandFromResourceAssembler
{
    public static ActivateAccountCommand ToCommandFromResource(ActivateAccountResource resource) =>
        new(resource.Token);
}
