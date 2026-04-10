using FULLSTACKFURY.EduSpace.API.IAM.Interfaces.ACL;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.OutboundServices.ACL;

public class ExternalIamService(IIamContextFacade iamContextFacade) : IExternalIamService
{
    public async Task<AccountId> CreateAccount(string username, string password, string role)
    {
        var accountId = await iamContextFacade.CreateAccount(username, password, role);
        if (accountId == 0) throw new Exception("Error creating the account");
        return new AccountId(accountId);
    }
}