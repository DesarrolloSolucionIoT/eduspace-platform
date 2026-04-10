using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Classroom;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Resource;

/// <summary>
///     Represents the data exposed by the resourceresource.
/// </summary>
/// <param name="Id">
///     The unique identifier of the  resource.
/// </param>
/// <param name="Name">
///     The name of the resource.
/// </param>
/// <param name="KindOfResource">
///     The kind of resource.
/// </param>
/// <param name="Classroom">
///     The <see cref="ClassroomResource" /> classroom of the resource.
/// </param>
public record ResourceResource(int Id, string Name, string KindOfResource, ClassroomResource? Classroom);