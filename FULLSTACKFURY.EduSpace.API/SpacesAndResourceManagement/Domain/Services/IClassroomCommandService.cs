using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Classroom;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;

/// <summary>
///     Represents the classroom command service in the EduSpace API.
/// </summary>
public interface IClassroomCommandService
{
    /// <summary>
    ///     Handles the create classroom command in the EduSpace API.
    /// </summary>
    /// <param name="command">
    ///     The <see cref="CreateClassroomCommand" /> command to handle.
    /// </param>
    /// <returns>
    ///     The created <see cref="Classroom" /> entity.
    /// </returns>
    Task<Classroom?> Handle(CreateClassroomCommand command);

    Task Handle(DeleteClassroomCommand command);
    Task<Classroom?> Handle(UpdateClassroomCommand command);
}