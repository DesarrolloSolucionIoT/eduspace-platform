using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.Internal.CommandServices;

/// <summary>
///     Represents a shared area command service for Shared Area entities
/// </summary>
/// <param name="sharedAreaRepository">
///     The repository for shared area entities
/// </param>
/// <param name="unitOfWork">
///     The unit of work for the repository
/// </param>
public class SharedAreaCommandService(ISharedAreaRepository sharedAreaRepository, IUnitOfWork unitOfWork)
    : ISharedAreaCommandService
{
    /// <inheritDoc />
    public async Task<SharedArea?> Handle(CreateSharedAreaCommand command)
    {
        if (await sharedAreaRepository.ExistsByNameAsync(command.Name))
            throw new Exception("Shared area with the same name already exists.");
        var sharedArea = new SharedArea(command);
        await sharedAreaRepository.AddAsync(sharedArea);
        await unitOfWork.CompleteAsync();
        return sharedArea;
    }

    public async Task Handle(DeleteSharedAreaCommand command)
    {
        var sharedArea = await sharedAreaRepository.FindByIdAsync(command.SharedAreaId);
        if (sharedArea == null) throw new ArgumentException("Meeting not found.");

        sharedAreaRepository.Remove(sharedArea);

        await unitOfWork.CompleteAsync();
    }

    public async Task<SharedArea?> Handle(UpdateSharedAreaCommand command)
    {
        var sharedArea = await sharedAreaRepository.FindByIdAsync(command.Id);
        if (sharedArea == null)
            throw new ArgumentException("Meeting not found.");

        sharedArea.UpdateName(command.Name);
        sharedArea.UpdateCapacity(command.Capacity);
        sharedArea.UpdateDescription(command.Description);

        sharedAreaRepository.Update(sharedArea);
        await unitOfWork.CompleteAsync();

        return sharedArea;
    }
}