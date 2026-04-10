using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;

/// <summary>
///     Resource Repository Interface
/// </summary>
public interface IResourceRepository : IBaseRepository<Resource>
{
    /// <summary>
    ///     Adds a new resource to the repository.
    /// </summary>
    /// <param name="classroomId">
    ///     The classroom id.
    /// </param>
    /// <returns>
    ///     A collection of classrooms that belong to the teacher.
    /// </returns>
    Task<IEnumerable<Resource>> FindByClassroomIdAsync(int classroomId);

    /// <summary>
    ///     Verifies if  a resource exists by its name.
    /// </summary>
    /// <param name="name">
    ///     The name of the resource to verify.
    /// </param>
    Task<bool> ExistsByNameAsync(string name);
}