using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.CommandServices;

/// <summary>
/// Generates an <see cref="ActivationToken"/>, persists it (atomically via UoW),
/// then sends an activation email (best-effort — Design Decision 3 &amp; 4).
/// </summary>
public class RequestAccountActivationCommandHandler(
    IActivationTokenRepository activationTokenRepository,
    IEmailService emailService,
    IUnitOfWork unitOfWork,
    ILogger<RequestAccountActivationCommandHandler> logger)
{
    public async Task Handle(RequestAccountActivationCommand command)
    {
        // Create token (returns entity + raw token — raw is NEVER stored)
        var (tokenEntity, rawToken) = ActivationToken.CreateNew(command.AccountId, TimeSpan.FromHours(24));

        await activationTokenRepository.AddAsync(tokenEntity);

        // Persist BEFORE email — atomicity guarantee for the DB write (Design Decision 3)
        await unitOfWork.CompleteAsync();

        logger.LogDebug(
            "Activation token persisted for account {AccountId}. Sending activation email to {Email}",
            command.AccountId, command.Email);

        // Email is best-effort: failure is logged + swallowed (Design Decision 4)
        try
        {
            await emailService.SendActivationEmailAsync(command.Email, command.FullName, rawToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "EmailService failed to deliver activation email to {Email}. " +
                "Account {AccountId} remains inactive. Reason: {Reason}",
                command.Email, command.AccountId, ex.Message);
        }
    }
}
