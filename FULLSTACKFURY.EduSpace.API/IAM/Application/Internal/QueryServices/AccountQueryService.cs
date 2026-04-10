using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Services;

namespace FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.QueryServices;

public class AccountQueryService(IAccountRepository accountRepository) : IAccountQueryService
{
    public async Task<Account?> Handle(GetAccountByIdQuery query)
    {
        return await accountRepository.FindByIdAsync(query.Id);
    }

    public async Task<Account?> Handle(GetAccountByUsernameQuery query)
    {
        return await accountRepository.FindByUsername(query.Username);
    }
}