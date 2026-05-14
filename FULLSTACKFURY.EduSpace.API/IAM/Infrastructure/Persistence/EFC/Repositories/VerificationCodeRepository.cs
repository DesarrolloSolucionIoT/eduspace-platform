using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Persistence.EFC.Repositories;

public class VerificationCodeRepository(AppDbContext context)
    : BaseRepository<VerificationCode>(context), IVerificationCodeRepository
{
    public async Task<VerificationCode?> FindActiveByAccountIdAndCodeAsync(int accountId, string code)
    {
        return await Context.Set<VerificationCode>()
            .FirstOrDefaultAsync(vc =>
                vc.AccountId == accountId &&
                vc.Code == code &&
                !vc.IsUsed &&
                vc.ExpirationDate > DateTime.UtcNow);
    }
}
