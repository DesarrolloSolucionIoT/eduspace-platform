using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Services;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.QueryServices;

public class TeacherProfileQueryService(ITeacherProfileRepository teacherProfileRepository) : ITeacherQueryService
{
    public async Task<IEnumerable<TeacherProfile>> Handle(GetAllTeachersProfileQuery query)
    {
        return await teacherProfileRepository.ListAsync();
    }

    public async Task<TeacherProfile?> Handle(GetTeacherProfileByIdQuery query)
    {
        return await teacherProfileRepository.FindByIdAsync(query.ProfileId);
    }
}