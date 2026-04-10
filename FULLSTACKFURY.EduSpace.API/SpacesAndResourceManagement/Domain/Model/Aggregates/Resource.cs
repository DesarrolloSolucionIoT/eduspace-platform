using System.ComponentModel.DataAnnotations;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Resource;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Aggregates;

/// <summary>
///     Represents a resource in the application.
/// </summary>
/// <remarks>
///     A resource is a physical object that can be used in a classroom.
/// </remarks>
public class Resource
{
    /// <summary>
    ///     Default constructor for the classroom entity
    /// </summary>
    public Resource()
    {
        Name = string.Empty;
        KindOfResource = string.Empty;
        Classroom = default!;
    }

    /// <param name="name">
    ///     The name of the resource
    /// </param>
    /// <param name="kind_of_resource">
    ///     The kind of resource
    /// </param>
    /// <param name="classroomId">
    ///     The classroom id
    /// </param>
    public Resource(string name, string kindOfResource, int classroomId) : this()
    {
        Name = name;
        KindOfResource = kindOfResource;
        ClassroomId = classroomId;
    }

    public Resource(CreateResourceCommand command) : this()
    {
        Name = command.Name;
        KindOfResource = command.KindOfResource;
        ClassroomId = command.ClassroomId;
    }

    public Resource(UpdateResourceCommand command) : this()
    {
        Id = command.Id;
        Name = command.Name;
        KindOfResource = command.KindOfResource;
        ClassroomId = command.ClassroomId;
    }

    [Key] public int Id { get; set; }

    public string Name { get; private set; }
    public string KindOfResource { get; private set; }
    public Classroom Classroom { get; internal set; }
    public int ClassroomId { get; private set; }

    public void UpdateName(string name)
    {
        if (!string.IsNullOrEmpty(name))
            Name = name;
    }

    public void UpdateKindOfResource(string kindOfResource)
    {
        if (!string.IsNullOrEmpty(kindOfResource))
            KindOfResource = kindOfResource;
    }

    public void UpdateClassroomId(int classroomId)
    {
        if (classroomId > 0 && classroomId != ClassroomId)
            ClassroomId = classroomId;
    }
}