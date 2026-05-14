using System.ComponentModel.DataAnnotations;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Resource;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;

/// <summary>
///     Represents a physical resource that belongs to a classroom.
/// </summary>
public class Resource
{
    /// <summary>EF Core parameterless constructor.</summary>
    public Resource()
    {
        Name = string.Empty;
        KindOfResource = string.Empty;
        Classroom = null!;
    }

    /// <summary>Primary constructor for new resources.</summary>
    public Resource(string name, string kindOfResource, int classroomId) : this()
    {
        ValidateName(name);
        ValidateKind(kindOfResource);
        ValidateClassroomId(classroomId);
        Name = name;
        KindOfResource = kindOfResource;
        ClassroomId = classroomId;
    }

    /// <summary>Constructor used when creating from a <see cref="CreateResourceCommand" />.</summary>
    public Resource(CreateResourceCommand command)
        : this(command.Name, command.KindOfResource, command.ClassroomId) { }

    [Key] public int Id { get; set; }

    public string Name { get; private set; }
    public string KindOfResource { get; private set; }

    /// <summary>Navigation property — loaded by EF on query, never mutated directly.</summary>
    public Classroom Classroom { get; private set; }

    public int ClassroomId { get; private set; }

    public void UpdateName(string name)
    {
        ValidateName(name);
        Name = name;
    }

    public void UpdateKindOfResource(string kindOfResource)
    {
        ValidateKind(kindOfResource);
        KindOfResource = kindOfResource;
    }

    public void UpdateClassroomId(int classroomId)
    {
        ValidateClassroomId(classroomId);
        if (classroomId != ClassroomId)
            ClassroomId = classroomId;
    }

    // ── private invariant guards ──────────────────────────────────────────────

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidResourceDataException("Resource name cannot be empty.");
    }

    private static void ValidateKind(string kind)
    {
        if (string.IsNullOrWhiteSpace(kind))
            throw new InvalidResourceDataException("KindOfResource cannot be empty.");
    }

    private static void ValidateClassroomId(int classroomId)
    {
        if (classroomId <= 0)
            throw new InvalidResourceDataException("ClassroomId must be a positive integer.");
    }
}
