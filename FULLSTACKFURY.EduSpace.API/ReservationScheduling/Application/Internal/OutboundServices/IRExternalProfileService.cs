namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.OutboundServices;

public interface IRExternalProfileService
{
    bool ValidateTeacherExistence(int teacherId);
    bool ValidateAdminIdExistence(int adminId);

    bool ValidateTeachersExistence(List<int> teacherIds);
}