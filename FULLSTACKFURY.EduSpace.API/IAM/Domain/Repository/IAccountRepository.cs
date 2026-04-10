using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;

public interface IAccountRepository : IBaseRepository<Account>
{
    Task<Account?> FindByUsername(string username);
    bool ExistsByUsername(string username);
}