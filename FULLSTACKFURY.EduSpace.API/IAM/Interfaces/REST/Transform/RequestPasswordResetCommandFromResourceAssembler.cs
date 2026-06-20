using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Resources;

namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Transform;

public static class RequestPasswordResetCommandFromResourceAssembler
{
    public static RequestPasswordResetCommand ToCommandFromResource(ForgotPasswordResource resource) =>
        new(resource.Email);
}
