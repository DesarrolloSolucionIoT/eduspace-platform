using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Transform;

public static class VerifyCodeCommandFromResourceAssembler
{
    public static VerifyCodeCommand ToCommandFromResource(VerifyCodeResource resource)
    {
        return new VerifyCodeCommand(resource.Username, resource.Code);
    }
}