using FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.ACL;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.OutboundServices.ACL;

public class ExternalProfileService(IProfilesContextFacade profilesContextFacade) : IExternalProfileService
{
    public Task<bool> VerifyProfileAsync(int teacherProfileId)
    {
        // IProfilesContextFacade.ValidateTeacherProfileIdExistence is synchronous (cross-BC constraint).
        // Wrapped in Task.FromResult so callers can stay async without blocking.
        return Task.FromResult(profilesContextFacade.ValidateTeacherProfileIdExistence(teacherProfileId));
    }
}
