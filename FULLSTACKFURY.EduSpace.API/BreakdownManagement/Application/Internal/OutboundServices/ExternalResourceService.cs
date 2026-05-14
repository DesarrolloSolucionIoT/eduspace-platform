using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.ACL;

namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Application.Internal.OutboundServices;

/// <summary>
/// ACL adapter — delegates resource existence checks to the SpacesAndResourceManagement BC facade.
/// </summary>
public class ExternalResourceService : IExternalResourceService
{
    private readonly ISpacesAndResourceManagementFacade _spacesFacade;

    public ExternalResourceService(ISpacesAndResourceManagementFacade spacesFacade)
    {
        _spacesFacade = spacesFacade;
    }

    /// <inheritdoc />
    public Task<bool> ValidateResourceExistsAsync(int resourceId)
    {
        var exists = _spacesFacade.ValidateResourceIdExistence(resourceId);
        return Task.FromResult(exists);
    }
}
