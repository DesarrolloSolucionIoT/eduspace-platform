using System.Net.Mime;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Services;
using FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Resources;
using FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available authentication endpoints")]
public class AuthenticationController(IAccountCommandService accountCommandService)
    : ControllerBase
{
    [HttpPost("sign-up")]
    [SwaggerOperation(
        Summary = "Sign up",
        Description = "Sign up with email and password",
        OperationId = "SignUp",
        Tags = new[] { "Authentication" })]
    [SwaggerResponse(StatusCodes.Status200OK, "The user was signed up.")]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource resource)
    {
        var signUpCommand = SignUpCommandFromResourceAssembler.ToCommandFromResource(resource);
        await accountCommandService.Handle(signUpCommand);
        return Ok(new { message = "User created succesfully" });
    }

    [AllowAnonymous]
    [HttpPost("sign-in")]
    [SwaggerOperation(
        Summary = "Sign in",
        Description = "Sign in with email and password",
        OperationId = "SignIn",
        Tags = new[] { "Authentication" })]
    [SwaggerResponse(StatusCodes.Status200OK, "The user was authenticated.", typeof(AuthenticatedAccountResource))]
    public async Task<IActionResult> SignIn([FromBody] SignInResource resource)
    {
        var signInCommand = SignInCommandFromResourceAssembler.ToCommandFromResource(resource);
        await accountCommandService.Handle(signInCommand);
        return Ok(new { message = "Verification code sent to your email." });
    }

    [AllowAnonymous]
    [HttpPost("verify-code")]
    [SwaggerOperation(
        Summary = "Verify Code and Sign In",
        Description = "Verifies the 2FA code and returns a JWT with complete user profile information.",
        OperationId = "VerifyCode",
        Tags = new[] { "Authentication" })]
    [SwaggerResponse(StatusCodes.Status200OK, "The user was authenticated.", typeof(AuthenticatedAccountResource))]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeResource resource)
    {
        var verifyCodeCommand = VerifyCodeCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await accountCommandService.Handle(verifyCodeCommand);
        var authenticatedAccountResource = AuthenticatedAccountResourceFromEntityAssembler
            .ToResourceFromEntity(
                result.account,
                result.token,
                result.profileId,
                result.teacherProfile,
                result.adminProfile,
                result.classrooms,
                result.meetings
            );
        return Ok(authenticatedAccountResource);
    }
}