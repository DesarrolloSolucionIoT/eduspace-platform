using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.CommandServices;

/// <summary>
/// Validates the raw activation token, activates the account,
/// and marks the token used — all in a single <see cref="IUnitOfWork.CompleteAsync"/> call (REQ-009).
/// </summary>
public class ActivateAccountCommandHandler(
    IActivationTokenRepository activationTokenRepository,
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
    ILogger<ActivateAccountCommandHandler> logger)
{
    public async Task Handle(ActivateAccountCommand command)
    {
        var hash = ActivationToken.ComputeHash(command.RawToken);

        var token = await activationTokenRepository.FindActiveByHashAsync(hash);

        if (token is null)
        {
            logger.LogWarning("Activation attempt with unknown token hash");
            throw new InvalidActivationTokenException();
        }

        // Discriminate between expired and already-used before calling IsValid()
        // so we can return specific error codes (Design Decision 6)
        if (token.UsedAt != null)
        {
            logger.LogWarning("Activation attempt with already-used token for account {AccountId}", token.AccountId);
            throw new ActivationTokenAlreadyUsedException();
        }

        if (token.ExpiresAt <= DateTime.UtcNow)
        {
            logger.LogWarning("Activation attempt with expired token for account {AccountId}", token.AccountId);
            throw new ActivationTokenExpiredException();
        }

        var account = await accountRepository.FindByIdAsync(token.AccountId);
        if (account is null)
        {
            logger.LogError("Token references non-existent account {AccountId}", token.AccountId);
            throw new AccountNotFoundException($"Account {token.AccountId} not found.");
        }

        account.Activate();
        token.MarkAsUsed();

        // Single UoW call — both mutations committed atomically (REQ-009)
        await unitOfWork.CompleteAsync();

        logger.LogInformation("Account {AccountId} successfully activated", account.Id);
    }
}
