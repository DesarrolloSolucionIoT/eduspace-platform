using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.CommandServices;

/// <summary>
/// Resolves the email to an account id via the teacher/admin profile repositories.
/// If no account matches, returns silently (anti-enumeration — no throw, no email).
/// Otherwise generates a <see cref="PasswordResetToken"/>, persists it (atomically via UoW),
/// then sends the reset email (best-effort). Mirrors <see cref="RequestAccountActivationCommandHandler"/>.
/// </summary>
public class RequestPasswordResetCommandHandler(
    IPasswordResetTokenRepository passwordResetTokenRepository,
    ITeacherProfileRepository teacherProfileRepository,
    IAdminProfileRepository adminProfileRepository,
    IEmailService emailService,
    IUnitOfWork unitOfWork,
    ILogger<RequestPasswordResetCommandHandler> logger)
{
    public async Task Handle(RequestPasswordResetCommand command)
    {
        var accountId = await teacherProfileRepository.FindAccountIdByEmailAsync(command.Email)
                        ?? await adminProfileRepository.FindAccountIdByEmailAsync(command.Email);

        // Anti-enumeration: unknown email is a silent no-op. The controller always
        // returns a generic 200 OK so callers cannot probe which emails exist.
        if (accountId is null)
        {
            logger.LogInformation("Password-reset requested for unknown email — silently ignored");
            return;
        }

        var (tokenEntity, rawToken) = PasswordResetToken.CreateNew(accountId.Value, TimeSpan.FromHours(1));

        await passwordResetTokenRepository.AddAsync(tokenEntity);

        // Persist BEFORE email — atomicity guarantee for the DB write.
        await unitOfWork.CompleteAsync();

        logger.LogDebug(
            "Password-reset token persisted for account {AccountId}. Sending reset email to {Email}",
            accountId.Value, command.Email);

        // Email is best-effort: failure is logged + swallowed.
        try
        {
            // The request only carries the email; use it as the display name fallback.
            await emailService.SendPasswordResetEmailAsync(command.Email, command.Email, rawToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "EmailService failed to deliver password-reset email to {Email}. Reason: {Reason}",
                command.Email, ex.Message);
        }
    }
}
