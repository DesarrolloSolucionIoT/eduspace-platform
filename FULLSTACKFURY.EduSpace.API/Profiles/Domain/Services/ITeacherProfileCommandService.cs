using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Commands;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Domain.Services;

public interface ITeacherProfileCommandService
{
    Task<TeacherProfile?> Handle(CreateTeacherProfileCommand command);
    Task<TeacherProfile?> Handle(UpdateTeacherProfileCommand command);
    Task Handle(DeleteTeacherProfileCommand command);
}