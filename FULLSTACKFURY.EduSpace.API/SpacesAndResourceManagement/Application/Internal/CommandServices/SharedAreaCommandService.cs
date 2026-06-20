using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.Internal.CommandServices;

/// <summary>
///     Handles command operations for <see cref="SharedArea" /> aggregates.
/// </summary>
public class SharedAreaCommandService(
    ISharedAreaRepository sharedAreaRepository,
    ISharedAreaReservationRepository sharedAreaReservationRepository,
    IUnitOfWork unitOfWork)
    : ISharedAreaCommandService
{
    /// <inheritdoc />
    public async Task<SharedArea?> Handle(CreateSharedAreaCommand command)
    {
        if (await sharedAreaRepository.ExistsByNameAsync(command.Name))
            throw new InvalidSharedAreaDataException($"A shared area named '{command.Name}' already exists.");

        var sharedArea = new SharedArea(command);
        await sharedAreaRepository.AddAsync(sharedArea);
        await unitOfWork.CompleteAsync();
        return sharedArea;
    }

    /// <inheritdoc />
    public async Task Handle(DeleteSharedAreaCommand command)
    {
        var sharedArea = await sharedAreaRepository.FindByIdAsync(command.SharedAreaId);
        if (sharedArea is null) throw new SharedAreaNotFoundException(command.SharedAreaId);

        sharedAreaRepository.Remove(sharedArea);
        await unitOfWork.CompleteAsync();
    }

    /// <inheritdoc />
    public async Task<SharedArea?> Handle(UpdateSharedAreaCommand command)
    {
        var sharedArea = await sharedAreaRepository.FindByIdAsync(command.Id);
        if (sharedArea is null) throw new SharedAreaNotFoundException(command.Id);

        sharedArea.Update(command.Name, command.Capacity, command.Description, command.ZoneId);

        sharedAreaRepository.Update(sharedArea);
        await unitOfWork.CompleteAsync();
        return sharedArea;
    }

    public async Task<SharedAreaReservation?> Handle(ReserveSharedAreaCommand command)
    {
        var sharedArea = await sharedAreaRepository.FindByIdAsync(command.SharedAreaId);
        if (sharedArea is null) throw new SharedAreaNotFoundException(command.SharedAreaId);

        var hasConflict = await sharedAreaReservationRepository
            .ExistsBySharedAreaAndDateTimeRangeAsync(
                command.SharedAreaId,
                command.ReservationDate,
                command.StartTime,
                command.EndTime);

        if (hasConflict)
            throw new SharedAreaReservationConflictException(
                $"The shared area '{sharedArea.Name}' is already reserved on {command.ReservationDate:yyyy-MM-dd} from {command.StartTime:hh\\:mm} to {command.EndTime:hh\\:mm}.");

        var reservation = new SharedAreaReservation(
            command.SharedAreaId,
            command.TeacherId,
            command.ReservationDate,
            command.StartTime,
            command.EndTime,
            command.Reason);

        await sharedAreaReservationRepository.AddAsync(reservation);
        await unitOfWork.CompleteAsync();
        return reservation;
    }
}
