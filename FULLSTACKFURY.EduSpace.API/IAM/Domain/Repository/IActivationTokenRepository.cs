using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;

public interface IActivationTokenRepository : IBaseRepository<ActivationToken>
{
    /// <summary>
    /// Returns the token matching the given hash, regardless of used/expired state.
    /// The handler is responsible for discriminating between invalid / expired / used
    /// to map to the correct error code (REQ-013, Design Decision 6).
    /// </summary>
    Task<ActivationToken?> FindByHashAsync(string hash);

    /// <summary>
    /// Hard-deletes every activation token for an account. Used by the IAM cascade
    /// triggered when a profile is deleted — activation_tokens FK is ON DELETE RESTRICT.
    /// </summary>
    Task DeleteAllForAccountAsync(int accountId);
}
