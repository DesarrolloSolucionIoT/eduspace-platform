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
/// <param name="ZoneId">
///     The ID of the zone to which the shared area belongs
/// </param>
public record SharedAreaResource(int Id, string Name, int Capacity, string Description, string? ZoneId = null);