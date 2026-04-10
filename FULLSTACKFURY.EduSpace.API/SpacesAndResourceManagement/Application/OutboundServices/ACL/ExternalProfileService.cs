using FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.ACL;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.OutboundServices.ACL;

public class ExternalProfileService(IProfilesContextFacade profilesContextFacade) : IExternalProfileService
{
    public bool VerifyProfile(int teacherProfileId)
    {
        return profilesContextFacade.ValidateTeacherProfileIdExistence(teacherProfileId);
    }
}