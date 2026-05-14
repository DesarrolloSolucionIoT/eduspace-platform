using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Tokens.JWT.Configuration;
using FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Resources;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using Microsoft.Extensions.Options;

namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Transform;

public static class AuthenticatedAccountResourceFromEntityAssembler
{
    public static AuthenticatedAccountResource ToResourceFromEntity(
        Account entity,
        string accessToken,
        string refreshToken,
        int accessTokenLifetimeMinutes,
        int? profileId,
        TeacherProfile? teacherProfile = null,
        AdminProfile? adminProfile = null,
        IEnumerable<Classroom>? classrooms = null,
        IEnumerable<Meeting>? meetings = null)
    {
        ProfileData? profileData = null;

        if (teacherProfile != null)
            profileData = new ProfileData(
                teacherProfile.Id,
                teacherProfile.ProfileName.FirstName,
                teacherProfile.ProfileName.LastName,
                teacherProfile.ProfilePrivateInformation.Email,
                teacherProfile.ProfilePrivateInformation.Dni,
                teacherProfile.ProfilePrivateInformation.Address,
                teacherProfile.ProfilePrivateInformation.Phone,
                teacherProfile.AdministratorId
            );
        else if (adminProfile != null)
            profileData = new ProfileData(
                adminProfile.Id,
                adminProfile.ProfileName.FirstName,
                adminProfile.ProfileName.LastName,
                adminProfile.ProfilePrivateInformation.Email,
                adminProfile.ProfilePrivateInformation.Dni,
                adminProfile.ProfilePrivateInformation.Address,
                adminProfile.ProfilePrivateInformation.Phone,
                adminProfile.Id
            );

        var classroomData = classrooms?.Select(c => new ClassroomData(c.Id, c.Name, c.Description));

        var meetingData = meetings?.Select(m => new MeetingData(
            m.Id,
            m.Title,
            m.Description,
            m.Date,
            m.StartTime,
            m.EndTime
        ));

        return new AuthenticatedAccountResource(
            entity.Id,
            profileId,
            entity.Username,
            entity.GetRole(),
            accessToken,
            refreshToken,
            accessTokenLifetimeMinutes * 60,
            profileData,
            classroomData,
            meetingData
        );
    }
}
