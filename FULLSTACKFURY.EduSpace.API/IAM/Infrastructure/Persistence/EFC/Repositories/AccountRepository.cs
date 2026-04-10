using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Persistence.EFC.Repositories;

public class AccountRepository(AppDbContext context) : BaseRepository<Account>(context), IAccountRepository
{
    public async Task<Account?> FindByUsername(string username)
    {
        return await Context.Set<Account>().FirstOrDefaultAsync(account => account.Username == username);
    }

    public bool ExistsByUsername(string username)
    {
        return Context.Set<Account>().Any(account => account.Username == username);
    }
}