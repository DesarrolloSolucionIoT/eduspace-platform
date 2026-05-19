using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Services;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.ACL.Services;

public class IamContextFacade(
    IAccountCommandService accountCommandService,
    IAccountQueryService accountQueryService,
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
    ILogger<IamContextFacade> logger) : IIamContextFacade
{
    public async Task<int> CreateAccount(string username, string password, string role)
    {
        var signUpCommand = new SignUpCommand(username, password, role);
        await accountCommandService.Handle(signUpCommand);

        var getUsernameQuery = new GetAccountByUsernameQuery(username);
        var result = await accountQueryService.Handle(getUsernameQuery);

        return result?.Id ?? 0;
    }

    /// <summary>
    /// Silently activates the account — loads, calls <c>Activate()</c>, persists.
    /// Used by <c>TeacherProfileCommandService</c> after creating the account (REQ-018).
    /// </summary>
    public async Task ActivateAccountAsync(int accountId)
    {
        var account = await accountRepository.FindByIdAsync(accountId);
        if (account is null)
        {
            logger.LogWarning("ActivateAccountAsync: account {AccountId} not found", accountId);
            throw new AccountNotFoundException($"Account {accountId} not found.");
        }

        account.Activate();
        await unitOfWork.CompleteAsync();

        logger.LogInformation("Account {AccountId} auto-activated (teacher path)", accountId);
    }

    /// <summary>
    /// Generates an activation token and sends the activation email.
    /// Used by <c>AdminProfileCommandService</c> after creating the account (REQ-017).
    /// </summary>
    public async Task RequestActivationEmailAsync(int accountId, string email, string fullName)
    {
        var command = new RequestAccountActivationCommand(accountId, email, fullName);
        await accountCommandService.Handle(command);

        logger.LogInformation(
            "Activation email requested for account {AccountId} → {Email}", accountId, email);
    }
}
