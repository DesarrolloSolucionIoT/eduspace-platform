namespace FULLSTACKFURY.EduSpace.API.BreakdownManagement.Application.Internal.OutboundServices;

/// <summary>
/// ACL outbound port — Profiles bounded context.
/// Allows BreakdownManagement to validate profile existence without a direct domain coupling.
/// </summary>
public interface IExternalProfileService
{
    /// <summary>
    /// Returns true if a profile with the given <paramref name="profileId"/> exists in the Profiles BC.
    /// </summary>
    Task<bool> ValidateProfileExistsAsync(int profileId);
}
