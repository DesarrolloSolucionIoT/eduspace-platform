using FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.ACL;

namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Application.Internal.OutboundServices;

/// <summary>
/// ACL adapter — delegates profile existence checks to the Profiles BC facade.
/// Wraps the synchronous facade method in a Task so the application layer stays async-uniform.
/// </summary>
public class ExternalProfileServices : IExternalProfileService
{
    private readonly IProfilesContextFacade _profilesContextFacade;

    public ExternalProfileServices(IProfilesContextFacade profilesContextFacade)
    {
        _profilesContextFacade = profilesContextFacade;
    }

    /// <inheritdoc />
    public Task<bool> ValidateProfileExistsAsync(int profileId)
    {
        // The Profiles facade is synchronous; wrap in a completed task to satisfy the async contract.
        var exists = _profilesContextFacade.ValidateAdminProfileIdExistence(profileId)
                     || _profilesContextFacade.ValidateTeacherProfileIdExistence(profileId);
        return Task.FromResult(exists);
    }
}
