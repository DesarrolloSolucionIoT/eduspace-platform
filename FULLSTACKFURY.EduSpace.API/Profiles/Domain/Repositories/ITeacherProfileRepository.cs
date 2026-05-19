using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;

public interface ITeacherProfileRepository : IBaseRepository<TeacherProfile>
{
    bool ExistsByTeacherProfileId(int teacherProfileId);
    Task<TeacherProfile?> FindByAccountIdAsync(int accountId);
    Task<int?> FindAccountIdByEmailAsync(string email);

    /// <summary>
    /// Returns the IAM account id linked to a profile via a projection query.
    /// See <see cref="IAdminProfileRepository.FindLinkedAccountIdAsync"/> for the rationale.
    /// </summary>
    Task<int?> FindLinkedAccountIdAsync(int profileId);
}
