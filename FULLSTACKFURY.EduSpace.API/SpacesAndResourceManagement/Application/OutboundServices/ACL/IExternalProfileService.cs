namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.OutboundServices.ACL;

public interface IExternalProfileService
{
    public bool VerifyProfile(int profileId);
}