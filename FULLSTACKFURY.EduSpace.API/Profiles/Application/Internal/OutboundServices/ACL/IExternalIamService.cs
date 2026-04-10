using FULLSTACKFURY.EduSpace.API.Profiles.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Application.Internal.OutboundServices.ACL;

public interface IExternalIamService
{
    Task<AccountId> CreateAccount(string username, string password, string role);
}