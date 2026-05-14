using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;

public interface IVerificationCodeRepository : IBaseRepository<VerificationCode>
{
    /// <summary>Returns the most recent active (unused, non-expired) code for the given account.</summary>
    Task<VerificationCode?> FindActiveByAccountIdAndCodeAsync(int accountId, string code);
}
