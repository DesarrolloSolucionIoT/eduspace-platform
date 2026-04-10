using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Queries;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Services;

public interface ITeacherQueryService
{
    Task<IEnumerable<TeacherProfile>> Handle(GetAllTeachersProfileQuery query);
    Task<TeacherProfile?> Handle(GetTeacherProfileByIdQuery query);
}