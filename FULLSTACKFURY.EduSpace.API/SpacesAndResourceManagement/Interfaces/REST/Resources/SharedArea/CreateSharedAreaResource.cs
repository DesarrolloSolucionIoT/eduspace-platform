namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.SharedArea;

/// <summary>
///     Represents the data required to create a new shared area.
/// </summary>
/// <param name="Name">
///     The name of the shared area
/// </param>
/// <param name="Capacity">
///     The capacity of the shared area
/// </param>
/// <param name="Description">
///     The description of the shared area
/// </param>
public record CreateSharedAreaResource(string Name, int Capacity, string Description);