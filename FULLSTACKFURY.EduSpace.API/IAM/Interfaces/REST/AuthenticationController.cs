using System.Net.Mime;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Services;
using FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Tokens.JWT.Configuration;
using FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Resources;
using FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available authentication endpoints")]
public class AuthenticationController(
    IAccountCommandService accountCommandService,
    IOptions<TokenSettings> tokenSettings)
    : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("sign-up")]
    [SwaggerOperation(
        Summary = "Sign up",
        Description = "Creates a new account.",
        OperationId = "SignUp",
        Tags = new[] { "Authentication" })]
    [SwaggerResponse(StatusCodes.Status200OK, "The user was signed up.")]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource resource)
    {
        var signUpCommand = SignUpCommandFromResourceAssembler.ToCommandFromResource(resource);
        await accountCommandService.Handle(signUpCommand);
        return Ok(new { message = "User created successfully." });
    }

    [AllowAnonymous]
    [HttpPost("sign-in")]
    [SwaggerOperation(
        Summary = "Sign in",
        Description = "Initiates sign-in; sends a 6-digit verification code to the account's email.",
        OperationId = "SignIn",
        Tags = new[] { "Authentication" })]
    [SwaggerResponse(StatusCodes.Status200OK, "Verification code sent.")]
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
        Description = "Verifies the 2FA code and returns JWT + refresh token with complete user profile.",
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
                result.accessToken,
                result.refreshToken,
                tokenSettings.Value.AccessTokenLifetimeMinutes,
                result.profileId,
                result.teacherProfile,
                result.adminProfile,
                result.classrooms,
                result.meetings
            );
        return Ok(authenticatedAccountResource);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [SwaggerOperation(
        Summary = "Refresh Access Token",
        Description = "Rotates the refresh token and returns a new access token.",
        OperationId = "RefreshToken",
        Tags = new[] { "Authentication" })]
    [SwaggerResponse(StatusCodes.Status200OK, "Token refreshed.", typeof(AuthenticatedAccountResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid or expired refresh token.")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenResource resource)
    {
        var command = new RefreshAccessTokenCommand(resource.RefreshToken);
        var (newAccessToken, newRefreshToken) = await accountCommandService.Handle(command);
        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = newRefreshToken,
            expiresIn = tokenSettings.Value.AccessTokenLifetimeMinutes * 60
        });
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    [SwaggerOperation(
        Summary = "Logout",
        Description = "Revokes the given refresh token.",
        OperationId = "Logout",
        Tags = new[] { "Authentication" })]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Logged out successfully.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Refresh token not found.")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenResource resource)
    {
        var command = new LogoutCommand(resource.RefreshToken);
        await accountCommandService.Handle(command);
        return NoContent();
    }
}
