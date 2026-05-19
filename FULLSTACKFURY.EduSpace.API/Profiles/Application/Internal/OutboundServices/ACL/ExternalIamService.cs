using FULLSTACKFURY.EduSpace.API.IAM.Interfaces.ACL;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.OutboundServices.ACL;

public class ExternalIamService(IIamContextFacade iamContextFacade) : IExternalIamService
{
    public async Task<AccountId> CreateAccount(string username, string password, string role)
    {
        var accountId = await iamContextFacade.CreateAccount(username, password, role);
        if (accountId == 0) throw new InvalidProfileDataException("Error creating the account in IAM context.");
        return new AccountId(accountId);
    }

    public Task ActivateAccountAsync(int accountId)
        => iamContextFacade.ActivateAccountAsync(accountId);

    public Task RequestActivationEmailAsync(int accountId, string email, string fullName)
        => iamContextFacade.RequestActivationEmailAsync(accountId, email, fullName);
}
