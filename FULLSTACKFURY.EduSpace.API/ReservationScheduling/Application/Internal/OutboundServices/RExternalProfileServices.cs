using FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.ACL;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Application.Internal.OutboundServices;

public class RExternalProfileServices(IProfilesContextFacade contextFacade) : IRExternalProfileService
{
    public bool ValidateTeacherExistence(int teacherId)
    {
        return contextFacade.ValidateTeacherProfileIdExistence(teacherId);
    }

    public bool ValidateAdminIdExistence(int adminid)
    {
        return contextFacade.ValidateAdminProfileIdExistence(adminid);
    }

    public bool ValidateTeachersExistence(List<int> teacherIds)
    {
        foreach (var teacherId in teacherIds)
            if (!ValidateTeacherExistence(teacherId))
                return false;

        return true;
    }
}