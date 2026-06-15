using System.Net.Mime;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;
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
        Description = "Validates credentials and returns JWT + refresh token directly.",
        OperationId = "SignIn",
        Tags = new[] { "Authentication" })]
    [SwaggerResponse(StatusCodes.Status200OK, "Authenticated.", typeof(AuthenticatedAccountResource))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Account not activated.")]
    public async Task<IActionResult> SignIn([FromBody] SignInResource resource)
    {
        try
        {
            var signInCommand = SignInCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await accountCommandService.Handle(signInCommand);

            var authenticatedAccountResource = AuthenticatedAccountResourceFromEntityAssembler.ToResourceFromEntity(
                result.account,
                result.accessToken,
                result.refreshToken,
                tokenSettings.Value.AccessTokenLifetimeMinutes,
                result.profileId,
                result.teacherProfile,
                result.adminProfile,
                result.classrooms,
                result.meetings);

            return Ok(authenticatedAccountResource);
        }
        catch (AccountNotActivatedException)
        {
            return StatusCode(403, new
            {
                code = "AccountNotActivated",
                message = "Tu cuenta aún no está activada. Revisá tu correo."
            });
        }
    }

    [AllowAnonymous]
    [HttpPost("activate")]
    [SwaggerOperation(
        Summary = "Activate account",
        Description = "Validates the activation token and activates the account.",
        OperationId = "ActivateAccount",
        Tags = new[] { "Authentication" })]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Account activated successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid, expired, or already-used token.")]
    public async Task<IActionResult> Activate([FromBody] ActivateAccountResource resource)
    {
        try
        {
            var command = ActivateAccountCommandFromResourceAssembler.ToCommandFromResource(resource);
            await accountCommandService.Handle(command);
            return NoContent();
        }
        catch (InvalidActivationTokenException)
        {
            return BadRequest(new
            {
                code = "InvalidToken",
                message = "El enlace de activación no es válido."
            });
        }
        catch (ActivationTokenExpiredException)
        {
            return BadRequest(new
            {
                code = "TokenExpired",
                message = "El enlace de activación expiró. Pedí uno nuevo."
            });
        }
        catch (ActivationTokenAlreadyUsedException)
        {
            return BadRequest(new
            {
                code = "TokenAlreadyUsed",
                message = "Este enlace ya fue usado."
            });
        }
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    [SwaggerOperation(
        Summary = "Forgot password",
        Description = "Requests a password-reset link. Always returns 200 OK regardless of whether the email exists (anti-enumeration).",
        OperationId = "ForgotPassword",
        Tags = new[] { "Authentication" })]
    [SwaggerResponse(StatusCodes.Status200OK, "If the email exists, a reset link was sent.")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordResource resource)
    {
        var command = RequestPasswordResetCommandFromResourceAssembler.ToCommandFromResource(resource);
        await accountCommandService.Handle(command);
        return Ok(new
        {
            message = "Si existe una cuenta con ese correo, te enviamos un enlace para restablecer tu contraseña."
        });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    [SwaggerOperation(
        Summary = "Reset password",
        Description = "Validates the password-reset token and updates the account password.",
        OperationId = "ResetPassword",
        Tags = new[] { "Authentication" })]
    [SwaggerResponse(StatusCodes.Status200OK, "Password reset successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid, expired, or already-used token.")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordResource resource)
    {
        try
        {
            var command = ResetPasswordCommandFromResourceAssembler.ToCommandFromResource(resource);
            await accountCommandService.Handle(command);
            return Ok(new { message = "Tu contraseña fue restablecida correctamente." });
        }
        catch (InvalidPasswordResetTokenException)
        {
            return BadRequest(new
            {
                code = "InvalidPasswordResetToken",
                message = "El enlace de recuperación no es válido."
            });
        }
        catch (PasswordResetTokenExpiredException)
        {
            return BadRequest(new
            {
                code = "PasswordResetTokenExpired",
                message = "El enlace de recuperación expiró. Pedí uno nuevo."
            });
        }
        catch (PasswordResetTokenAlreadyUsedException)
        {
            return BadRequest(new
            {
                code = "PasswordResetTokenAlreadyUsed",
                message = "Este enlace de recuperación ya fue usado."
            });
        }
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
