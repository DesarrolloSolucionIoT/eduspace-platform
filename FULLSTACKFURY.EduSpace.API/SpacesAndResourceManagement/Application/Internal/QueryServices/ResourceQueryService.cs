using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.Internal.QueryServices;

/// <summary>
///     The resource query service
/// </summary>
/// <param name="resourceRepository">
///     The resource repository
/// </param>
public class ResourceQueryService(IResourceRepository resourceRepository) : IResourceQueryService
{
    // <inheritdoc />
    public async Task<Resource?> Handle(GetResourceByIdQuery query)
    {
        return await resourceRepository.FindByIdAsync(query.ResourceId);
    }

    // <inheritdoc />
    public async Task<IEnumerable<Resource>> Handle(GetAllResourcesQuery query)
    {
        return await resourceRepository.ListAsync();
    }

    // <inheritdoc />
    public async Task<IEnumerable<Resource>> Handle(GetAllResourcesByClassroomIdQuery query)
    {
        return await resourceRepository.FindByClassroomIdAsync(query.ClassroomId);
    }
}