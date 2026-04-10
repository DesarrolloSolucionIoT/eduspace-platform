using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;

public interface ITeacherProfileRepository : IBaseRepository<TeacherProfile>
{
    Task<IEnumerable<TeacherProfile>> FindAllTeachersByAdministratorIdAsync(int id);
    bool ExistsByTeacherProfileId(int teacherProfileId);
}