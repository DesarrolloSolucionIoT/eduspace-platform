using System.ComponentModel.DataAnnotations;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Classroom;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.ValueObjects;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;

/// <summary>
///     Classroom aggregate root entity.
/// </summary>
public class Classroom
{
    /// <summary>EF Core parameterless constructor.</summary>
    public Classroom()
    {
        Name = string.Empty;
        Description = string.Empty;
        TeacherId = default!;
    }

    /// <summary>Primary constructor for new classrooms.</summary>
    public Classroom(string name, string description, int teacherId) : this()
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidateTeacherId(teacherId);
        Name = name;
        Description = description;
        TeacherId = new TeacherId(teacherId);
    }

    /// <summary>Constructor used when creating from a <see cref="CreateClassroomCommand" />.</summary>
    public Classroom(CreateClassroomCommand command)
        : this(command.Name, command.Description, command.TeacherId) { }

    [Key] public int Id { get; private set; }

    public string Name { get; private set; }
    public string Description { get; private set; }

    public TeacherId TeacherId { get; private set; }

    public ICollection<Resource> Resources { get; private set; } = new List<Resource>();

    /// <summary>
    ///     Updates all mutable fields of the classroom. The caller is responsible for
    ///     verifying that <paramref name="teacherId" /> resolves to a valid teacher BEFORE
    ///     calling this method.
    /// </summary>
    public void Update(string name, string description, int teacherId)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidateTeacherId(teacherId);
        Name = name;
        Description = description;
        TeacherId = new TeacherId(teacherId);
    }

    // ── private invariant guards ──────────────────────────────────────────────

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidClassroomDataException("Classroom name cannot be empty.");
        if (name.Length > 100)
            throw new InvalidClassroomDataException("Classroom name cannot exceed 100 characters.");
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new InvalidClassroomDataException("Classroom description cannot be empty.");
        if (description.Length > 500)
            throw new InvalidClassroomDataException("Classroom description cannot exceed 500 characters.");
    }

    private static void ValidateTeacherId(int teacherId)
    {
        if (teacherId <= 0)
            throw new InvalidClassroomDataException("TeacherId must be a positive integer.");
    }
}
