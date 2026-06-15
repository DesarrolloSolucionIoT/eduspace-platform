using FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.CommandServices;

/// <summary>
/// Validates the raw password-reset token, updates the account password hash,
/// and marks the token used — all in a single <see cref="IUnitOfWork.CompleteAsync"/> call.
/// Mirrors <see cref="ActivateAccountCommandHandler"/>.
/// </summary>
public class ResetPasswordCommandHandler(
    IPasswordResetTokenRepository passwordResetTokenRepository,
    IAccountRepository accountRepository,
    IHashingService hashingService,
    IUnitOfWork unitOfWork,
    ILogger<ResetPasswordCommandHandler> logger)
{
    public async Task Handle(ResetPasswordCommand command)
    {
        var hash = PasswordResetToken.ComputeHash(command.RawToken);

        var token = await passwordResetTokenRepository.FindByHashAsync(hash);

        if (token is null)
        {
            logger.LogWarning("Password-reset attempt with unknown token hash");
            throw new InvalidPasswordResetTokenException();
        }

        // Discriminate between used and expired before mutating so we can return
        // specific error codes (mirrors the activation flow).
        if (token.UsedAt != null)
        {
            logger.LogWarning("Password-reset attempt with already-used token for account {AccountId}", token.AccountId);
            throw new PasswordResetTokenAlreadyUsedException();
        }

        if (token.ExpiresAt <= DateTime.UtcNow)
        {
            logger.LogWarning("Password-reset attempt with expired token for account {AccountId}", token.AccountId);
            throw new PasswordResetTokenExpiredException();
        }

        var account = await accountRepository.FindByIdAsync(token.AccountId);
        if (account is null)
        {
            logger.LogError("Password-reset token references non-existent account {AccountId}", token.AccountId);
            throw new AccountNotFoundException($"Account {token.AccountId} not found.");
        }

        account.UpdatePasswordHash(hashingService.HashPassword(command.NewPassword));
        token.MarkAsUsed();

        // Single UoW call — both mutations committed atomically.
        await unitOfWork.CompleteAsync();

        logger.LogInformation("Password successfully reset for account {AccountId}", account.Id);
    }
}
