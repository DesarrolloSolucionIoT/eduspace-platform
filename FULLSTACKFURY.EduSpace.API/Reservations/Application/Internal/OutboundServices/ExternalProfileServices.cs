using FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.ACL;

namespace FULLSTACKFURY.EduSpace.API.EventsScheduling.Application.Internal.OutboundServices;

public class ExternalProfileServices(IProfilesContextFacade contextFacade) : IExternalProfileService
{
    public bool ValidateTeacherIdExistence(int teacherId)
    {
        return contextFacade.ValidateTeacherProfileIdExistence(teacherId);
    }
}