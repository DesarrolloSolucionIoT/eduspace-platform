using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;

/// <summary>
///     Represents a repository for shared areas in the EduSpace API.
/// </summary>
public interface ISharedAreaRepository : IBaseRepository<SharedArea>
{
    /// <summary>
    ///     Verify if a shared area with specified title exists.
    /// </summary>
    /// <param name="name">
    ///     The name of the shared area to verify.
    /// </param>
    /// <returns>
    ///     True if the shared area exists, otherwise false.
    /// </returns>
    Task<bool> ExistsByNameAsync(string name);
}