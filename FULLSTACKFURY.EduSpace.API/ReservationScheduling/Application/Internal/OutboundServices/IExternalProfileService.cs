namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.OutboundServices;

public interface IExternalProfileService
{
    Task<bool> ValidateTeacherExistenceAsync(int teacherId);
    Task<bool> ValidateAdminIdExistenceAsync(int adminId);
    Task<bool> ValidateTeachersExistenceAsync(List<int> teacherIds);
}
