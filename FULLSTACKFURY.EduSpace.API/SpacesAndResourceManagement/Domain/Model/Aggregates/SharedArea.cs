using System.ComponentModel.DataAnnotations;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.SharedArea;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;

/// <summary>
///     Represents a shared area in the school (e.g., library, auditorium).
/// </summary>
public class SharedArea
{
    private const int MaxCapacity = 1000;

    /// <summary>EF Core parameterless constructor.</summary>
    public SharedArea()
    {
        Name = string.Empty;
        Description = string.Empty;
        ZoneId = null;
    }

    /// <summary>Primary constructor for new shared areas.</summary>
    public SharedArea(string name, int capacity, string description, string? zoneId = null) : this()
    {
        ValidateName(name);
        ValidateCapacity(capacity);
        ValidateDescription(description);
        Name = name;
        Capacity = capacity;
        Description = description;
        ZoneId = zoneId;      
    }

    /// <summary>Constructor used when creating from a <see cref="CreateSharedAreaCommand" />.</summary>
    public SharedArea(CreateSharedAreaCommand command)
        : this(command.Name, command.Capacity, command.Description, command.ZoneId) { }

    [Key] public int Id { get; private set; }
    public string Name { get; private set; }
    public int Capacity { get; private set; }
    public string Description { get; private set; }

    /// <summary>
    /// Gets the zoneId to work with the IoT monitoring system.
    /// </summary>
    public string? ZoneId { get; private set; }

    /// <summary>
    ///     Updates all mutable fields of the shared area.
    /// </summary>
    public void Update(string name, int capacity, string description, string? zoneId = null)
    {
        ValidateName(name);
        ValidateCapacity(capacity);
        ValidateDescription(description);
        Name = name;
        Capacity = capacity;
        Description = description;
        ZoneId = zoneId;
    }

    public void UpdateName(string name)
    {
        ValidateName(name);
        Name = name;
    }

    public void UpdateDescription(string description)
    {
        ValidateDescription(description);
        Description = description;
    }

    public void UpdateCapacity(int capacity)
    {
        ValidateCapacity(capacity);
        Capacity = capacity;
    }

    // ── private invariant guards ──────────────────────────────────────────────

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidSharedAreaDataException("Shared area name cannot be empty.");
    }

    private static void ValidateCapacity(int capacity)
    {
        if (capacity <= 0)
            throw new InvalidSharedAreaDataException("Capacity must be greater than zero.");
        if (capacity > MaxCapacity)
            throw new InvalidSharedAreaDataException($"Capacity cannot exceed {MaxCapacity}.");
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new InvalidSharedAreaDataException("Shared area description cannot be empty.");
    }
}
