using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Resource;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.Internal.CommandServices;

/// <summary>
///     Handles command operations for <see cref="Resource" /> aggregates.
/// </summary>
public class ResourceCommandService(
    IClassroomRepository classroomRepository,
    IResourceRepository resourceRepository,
    IUnitOfWork unitOfWork) : IResourceCommandService
{
    public async Task<Resource?> Handle(CreateResourceCommand command)
    {
        var classroom = await classroomRepository.FindByIdAsync(command.ClassroomId);
        if (classroom is null) throw new ClassroomNotFoundException(command.ClassroomId);

        if (await resourceRepository.ExistsByNameAndClassroomIdAsync(command.Name, command.ClassroomId))
            throw new InvalidResourceDataException($"A resource named '{command.Name}' already exists in this classroom.");

        var resource = new Resource(command);
        await resourceRepository.AddAsync(resource);
        await unitOfWork.CompleteAsync();
        // NOTE: resource.Classroom is NOT assigned here on purpose.
        // The FK (ClassroomId) is persisted correctly; the nav property will be
        // populated by EF on the next tracked query. Assigning it after CompleteAsync
        // would be a silent no-op on the DB (post-save mutation bug).
        return resource;
    }

    public async Task Handle(DeleteResourceCommand command)
    {
        var resource = await resourceRepository.FindByIdAsync(command.ResourceId);
        if (resource is null) throw new ResourceNotFoundException(command.ResourceId);

        resourceRepository.Remove(resource);
        await unitOfWork.CompleteAsync();
    }

    public async Task<Resource?> Handle(UpdateResourceCommand command)
    {
        var resource = await resourceRepository.FindByIdAsync(command.Id);
        if (resource is null) throw new ResourceNotFoundException(command.Id);

        resource.UpdateName(command.Name);
        resource.UpdateKindOfResource(command.KindOfResource);
        resource.UpdateClassroomId(command.ClassroomId);

        resourceRepository.Update(resource);
        await unitOfWork.CompleteAsync();
        return resource;
    }
}
