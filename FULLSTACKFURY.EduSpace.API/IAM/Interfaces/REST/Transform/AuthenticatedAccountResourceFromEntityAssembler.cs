using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Resources;
using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.ReservationScheduling.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;

namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Transform;

public static class AuthenticatedAccountResourceFromEntityAssembler
{
    public static AuthenticatedAccountResource ToResourceFromEntity(
        Account entity,
        string token,
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
                teacherProfile.ProfilePrivateInformation.ObtainEmail,
                teacherProfile.ProfilePrivateInformation.ObtainDni,
                teacherProfile.ProfilePrivateInformation.Address,
                teacherProfile.ProfilePrivateInformation.Phone,
                teacherProfile.AdministratorId
            );
        else if (adminProfile != null)
            profileData = new ProfileData(
                adminProfile.Id,
                adminProfile.ProfileName.FirstName,
                adminProfile.ProfileName.LastName,
                adminProfile.ProfilePrivateInformation.ObtainEmail,
                adminProfile.ProfilePrivateInformation.ObtainDni,
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
            token,
            profileData,
            classroomData,
            meetingData
        );
    }
}