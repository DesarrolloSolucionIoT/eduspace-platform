using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Queries;

namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Services;

public interface IAccountQueryService
{
    Task<Account?> Handle(GetAccountByIdQuery query);
    Task<Account?> Handle(GetAccountByUsernameQuery query);
}