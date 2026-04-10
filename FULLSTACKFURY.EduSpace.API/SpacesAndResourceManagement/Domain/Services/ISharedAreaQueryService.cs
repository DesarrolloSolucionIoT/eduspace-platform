using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;

/// <summary>
///     Represents the shared area query service in the EduSpace API.
/// </summary>
public interface ISharedAreaQueryService
{
    /// <summary>
    ///     Handles the get shared area by id query in the EduSpace API.
    /// </summary>
    /// <param name="query">
    ///     The <see cref="GetSharedAreaByIdQuery" /> query to handle.
    /// </param>
    /// <returns>
    ///     The <see cref="SharedArea" /> entity.
    /// </returns>
    Task<SharedArea?> Handle(GetSharedAreaByIdQuery query);

    /// <summary>
    ///     Handles the get all shared area query in the EduSpace API.
    /// </summary>
    /// <param name="query">
    ///     The <see cref="GetAllSharedAreasQuery" /> query to handle.
    /// </param>
    /// <returns>
    ///     A collection of all shared area in the platform.
    /// </returns>
    Task<IEnumerable<SharedArea>> Handle(GetAllSharedAreasQuery query);
}