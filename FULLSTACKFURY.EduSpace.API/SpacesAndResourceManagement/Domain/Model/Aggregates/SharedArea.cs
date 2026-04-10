using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;

/// <summary>
///     Represents a shared area in the application.
/// </summary>
/// <remarks>
///     This class is used to represent a shared area in the application.
/// </remarks>
public class SharedArea
{
    /// <summary>
    ///     Default constructor for the shared area entity
    /// </summary>
    public SharedArea()
    {
        Name = string.Empty;
        Description = string.Empty;
    }

    /// <param name="name">
    ///     The name of the shared area
    /// </param>
    /// <param name="capacity">
    ///     The capacity of the shared area
    /// </param>
    /// <param name="description">
    ///     The description of the shared area
    /// </param>
    public SharedArea(string name, int capacity, string description) : this()
    {
        Name = name;
        Capacity = capacity;
        Description = description;
    }

    public SharedArea(CreateSharedAreaCommand command) : this()
    {
        Name = command.Name;
        Capacity = command.Capacity;
        Description = command.Description;
    }

    public SharedArea(UpdateSharedAreaCommand command) : this()
    {
        Id = command.Id;
        Name = command.Name;
        Capacity = command.Capacity;
        Description = command.Description;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public int Capacity { get; set; }
    public string Description { get; set; }

    public void UpdateName(string name)
    {
        if (!string.IsNullOrEmpty(name))
            Name = name;
    }

    public void UpdateDescription(string description)
    {
        if (!string.IsNullOrEmpty(description))
            Description = description;
    }

    public void UpdateCapacity(int capacity)
    {
        if (capacity > 0)
            Capacity = capacity;
    }
}