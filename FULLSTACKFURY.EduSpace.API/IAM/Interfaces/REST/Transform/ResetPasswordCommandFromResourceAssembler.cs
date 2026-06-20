using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Transform;

public static class ResetPasswordCommandFromResourceAssembler
{
    public static ResetPasswordCommand ToCommandFromResource(ResetPasswordResource resource) =>
        new(resource.Token, resource.NewPassword);
}
