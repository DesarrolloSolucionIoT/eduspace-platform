using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;

public interface IAdminProfileRepository : IBaseRepository<AdminProfile>
{
    bool ExistsByAdminProfileId(int adminProfileId);
    Task<AdminProfile?> FindByAccountIdAsync(int accountId);
    Task<int?> FindAccountIdByEmailAsync(string email);

    /// <summary>
    /// Returns the IAM account id linked to a profile via a projection query.
    /// Use this instead of <c>profile.AccountId.Id</c> after <c>FindByIdAsync</c>:
    /// EF auto-discovered the <c>AccountId</c> record as a separate entity, so the
    /// navigation property is not populated by default and reading it throws NRE.
    /// </summary>
    Task<int?> FindLinkedAccountIdAsync(int profileId);
}
