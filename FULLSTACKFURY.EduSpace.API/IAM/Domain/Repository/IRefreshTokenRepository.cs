using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;

public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken?> FindByHashAsync(string hash);
    Task<IEnumerable<RefreshToken>> FindActiveByAccountIdAsync(int accountId);
    Task RevokeAllForAccountAsync(int accountId);
}
