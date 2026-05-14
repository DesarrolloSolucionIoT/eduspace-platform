using FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.ACL;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.OutboundServices;

public class ExternalProfileService(IProfilesContextFacade contextFacade) : IExternalProfileService
{
    public Task<bool> ValidateTeacherExistenceAsync(int teacherId)
    {
        return Task.FromResult(contextFacade.ValidateTeacherProfileIdExistence(teacherId));
    }

    public Task<bool> ValidateAdminIdExistenceAsync(int adminId)
    {
        return Task.FromResult(contextFacade.ValidateAdminProfileIdExistence(adminId));
    }

    public async Task<bool> ValidateTeachersExistenceAsync(List<int> teacherIds)
    {
        foreach (var teacherId in teacherIds)
            if (!await ValidateTeacherExistenceAsync(teacherId))
                return false;

        return true;
    }
}
