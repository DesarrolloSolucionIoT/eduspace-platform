using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;

public interface IResourceQueryService
{
    /// <summary>
    ///     Handles the get resource by id query in the EduSpace API.
    /// </summary>
    /// <param name="query">
    ///     The <see cref="GetResourceByIdQuery" /> query to handle.
    /// </param>
    /// <returns>
    ///     The <see cref="Resource" /> entity.
    /// </returns>
    Task<Resource?> Handle(GetResourceByIdQuery query);

    /// <summary>
    ///     Handles the get all resource query in the EduSpace API.
    /// </summary>
    /// <param name="query">
    ///     The <see cref="GetAllResourcesQuery" /> query to handle.
    /// </param>
    /// <returns>
    ///     A collection of all classrooms in the platform.
    /// </returns>
    Task<IEnumerable<Resource>> Handle(GetAllResourcesQuery query);

    /// <summary>
    ///     Handles the get all resources by classroom id query in the EduSpace API.
    /// </summary>
    /// <param name="query">
    ///     The <see cref="GetAllResourcesByClassroomIdQuery" /> query to handle.
    /// </param>
    /// <returns>
    ///     A collection of resource  that belong to the classroom.
    /// </returns>
    Task<IEnumerable<Resource>> Handle(GetAllResourcesByClassroomIdQuery query);
}