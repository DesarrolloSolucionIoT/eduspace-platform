using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.ACL.Services;

public class SpacesAndResourceManagementFacade(IClassroomRepository classroomRepository, IResourceRepository resourceRepository)
    : ISpacesAndResourceManagementFacade
{
    public bool ValidateClassroomIdExistence(int classroomId)
    {
        // ISpacesAndResourceManagementFacade is synchronous by cross-BC contract.
        // Bridge to the async repository method without blocking the thread pool by
        // running the async work via GetAwaiter().GetResult() inside a sync context.
        return classroomRepository.ExistsByClassroomIdAsync(classroomId).GetAwaiter().GetResult();
    }

    public bool ValidateResourceIdExistence(int resourceId)
    {
        return resourceRepository.ExistsByIdAsync(resourceId).GetAwaiter().GetResult();
    }
}
