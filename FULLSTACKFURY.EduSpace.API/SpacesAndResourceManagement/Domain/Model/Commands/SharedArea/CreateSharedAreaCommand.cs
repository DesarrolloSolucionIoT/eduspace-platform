namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;

/// <summary>
///     Command to create a new shared area.
/// </summary>
/// <param name="Name">
///     The name of the shared area to create.
/// </param>
/// <param name="Capacity">
///     The capacity of shared area.
/// </param>
/// <param name="Description">
///     The description of the shared area to create.
/// </param>
public record CreateSharedAreaCommand(string Name, int Capacity, string Description);