using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Repository;

public interface IPasswordResetTokenRepository : IBaseRepository<PasswordResetToken>
{
    /// <summary>
    /// Returns the token matching the given hash, regardless of used/expired state.
    /// The handler is responsible for discriminating between invalid / expired / used
    /// to map to the correct error code (mirrors <see cref="IActivationTokenRepository"/>).
    /// </summary>
    Task<PasswordResetToken?> FindByHashAsync(string hash);
}
