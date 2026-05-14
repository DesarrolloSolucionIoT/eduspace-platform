using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Commands;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;

namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Services;

public interface IAccountCommandService
{
    Task Handle(SignUpCommand command);
    Task Handle(SignInCommand command);

    Task<(Account account, string accessToken, string refreshToken, int? profileId, TeacherProfile? teacherProfile,
        AdminProfile? adminProfile, IEnumerable<Classroom>? classrooms, IEnumerable<Meeting>? meetings)>
        Handle(VerifyCodeCommand command);

    Task<(string newAccessToken, string newRefreshToken)> Handle(RefreshAccessTokenCommand command);
    Task Handle(LogoutCommand command);
}
