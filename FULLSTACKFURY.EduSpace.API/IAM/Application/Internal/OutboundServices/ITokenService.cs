using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;

namespace FULLSTACKFURY.EduSpace.API.IAM.Application.Internal.OutboundServices;

public interface ITokenService
{
    string GenerateToken(Account account);
    Task<int?> ValidateToken(string token);
}