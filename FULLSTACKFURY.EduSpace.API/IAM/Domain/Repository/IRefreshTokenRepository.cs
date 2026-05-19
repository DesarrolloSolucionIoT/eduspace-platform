using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;

public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken?> FindByHashAsync(string hash);
    Task<IEnumerable<RefreshToken>> FindActiveByAccountIdAsync(int accountId);
    Task RevokeAllForAccountAsync(int accountId);

    /// <summary>
    /// Hard-deletes every refresh token for an account. Used by the IAM cascade
    /// triggered when a profile is deleted — refresh_tokens FK is ON DELETE RESTRICT.
    /// </summary>
    Task DeleteAllForAccountAsync(int accountId);
}
