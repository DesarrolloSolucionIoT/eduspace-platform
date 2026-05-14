using FULLSTACKFURY.EduSpace.API.Shared.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.OutboundServices.ACL;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Classroom;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Repositories;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Application.Internal.CommandServices;

/// <summary>
///     Handles command operations for <see cref="Classroom" /> aggregates.
/// </summary>
public class ClassroomCommandService(
    IClassroomRepository classroomRepository,
    IExternalProfileService profileService,
    IUnitOfWork unitOfWork) : IClassroomCommandService
{
    /// <inheritdoc />
    public async Task<Classroom?> Handle(CreateClassroomCommand command)
    {
        if (!await profileService.VerifyProfileAsync(command.TeacherId))
            throw new TeacherNotFoundForClassroomException(command.TeacherId);

        if (await classroomRepository.ExistsByNameAsync(command.Name))
            throw new InvalidClassroomDataException($"A classroom named '{command.Name}' already exists.");

        var classroom = new Classroom(command);
        await classroomRepository.AddAsync(classroom);
        await unitOfWork.CompleteAsync();
        return classroom;
    }

    /// <inheritdoc />
    public async Task Handle(DeleteClassroomCommand command)
    {
        var classroom = await classroomRepository.FindByIdAsync(command.ClassroomId);
        if (classroom is null) throw new ClassroomNotFoundException(command.ClassroomId);

        classroomRepository.Remove(classroom);
        await unitOfWork.CompleteAsync();
    }

    /// <inheritdoc />
    public async Task<Classroom?> Handle(UpdateClassroomCommand command)
    {
        var classroom = await classroomRepository.FindByIdAsync(command.ClassroomId);
        if (classroom is null) throw new ClassroomNotFoundException(command.ClassroomId);

        // Verify teacher existence before touching the aggregate.
        if (!await profileService.VerifyProfileAsync(command.TeacherId))
            throw new TeacherNotFoundForClassroomException(command.TeacherId);

        classroom.Update(command.Name, command.Description, command.TeacherId);

        classroomRepository.Update(classroom);
        await unitOfWork.CompleteAsync();
        return classroom;
    }
}
