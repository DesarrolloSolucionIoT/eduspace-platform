using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using FULLSTACKFURY.EduSpace.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Persistence.EFC.Repositories;

public class PasswordResetTokenRepository(AppDbContext context)
    : BaseRepository<PasswordResetToken>(context), IPasswordResetTokenRepository
{
    public async Task<PasswordResetToken?> FindByHashAsync(string hash)
    {
        return await Context.Set<PasswordResetToken>()
            .FirstOrDefaultAsync(prt => prt.TokenHash == hash);
    }
}
