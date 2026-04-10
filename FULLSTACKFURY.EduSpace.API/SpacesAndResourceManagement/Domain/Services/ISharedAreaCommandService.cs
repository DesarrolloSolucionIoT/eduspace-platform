using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;

/// <summary>
///     Represents the shared area command service in the EduSpace API.
/// </summary>
public interface ISharedAreaCommandService
{
    /// <summary>
    ///     Handles the create shared area command in the EduSpace API.
    /// </summary>
    /// <param name="command">
    ///     The <see cref="CreateSharedAreaCommand" /> command to handle.
    /// </param>
    /// <returns>
    ///     The created <see cref="SharedArea" /> entity.
    /// </returns>
    Task<SharedArea?> Handle(CreateSharedAreaCommand command);

    Task Handle(DeleteSharedAreaCommand command);
    Task<SharedArea?> Handle(UpdateSharedAreaCommand command);
}