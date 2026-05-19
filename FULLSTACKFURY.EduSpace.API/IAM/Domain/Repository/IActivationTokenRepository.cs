using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;

public interface IActivationTokenRepository : IBaseRepository<ActivationToken>
{
    Task<ActivationToken?> FindActiveByHashAsync(string hash);
}
