namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.OutboundServices.ACL;

public interface IExternalProfileService
{
    Task<bool> VerifyProfileAsync(int profileId);
}
