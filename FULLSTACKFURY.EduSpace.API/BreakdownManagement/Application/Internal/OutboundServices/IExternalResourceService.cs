namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Application.Internal.OutboundServices;

/// <summary>
/// ACL outbound port — SpacesAndResourceManagement bounded context.
/// Allows BreakdownManagement to validate resource existence without a direct domain coupling.
/// </summary>
public interface IExternalResourceService
{
    /// <summary>
    /// Returns true if a resource with the given <paramref name="resourceId"/> exists in the
    /// SpacesAndResourceManagement BC.
    /// </summary>
    Task<bool> ValidateResourceExistsAsync(int resourceId);
}
