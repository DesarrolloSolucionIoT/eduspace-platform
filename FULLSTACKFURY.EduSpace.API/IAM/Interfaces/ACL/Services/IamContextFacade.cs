using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Services;

namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.ACL.Services;

public class IamContextFacade(
    IAccountCommandService accountCommandService,
    IAccountQueryService accountQueryService) : IIamContextFacade
{
    public async Task<int> CreateAccount(string username, string password, string role)
    {
        var signUpCommand = new SignUpCommand(username, password, role);
        await accountCommandService.Handle(signUpCommand);

        var getUsernameQuery = new GetAccountByUsernameQuery(username);
        var result = await accountQueryService.Handle(getUsernameQuery);

        return result?.Id ?? 0;
    }
}