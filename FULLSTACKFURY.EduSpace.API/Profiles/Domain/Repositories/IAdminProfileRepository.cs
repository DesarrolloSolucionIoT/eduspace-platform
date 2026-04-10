using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;

public interface IAdminProfileRepository : IBaseRepository<AdminProfile>
{
    bool ExistsByAdminProfileId(int adminProfileId);
}