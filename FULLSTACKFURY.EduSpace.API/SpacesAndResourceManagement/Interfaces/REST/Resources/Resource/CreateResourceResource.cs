namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Resource;

/// <summary>
///     The resource to create a new resource
/// </summary>
/// <param name="Name">
///     The name of the resource
/// </param>
/// <param name="KindOfResource">
///     The kind of resource
/// </param>
/// <param name="ClassroomId">
///     The id of the Classroom
/// </param>
public record CreateResourceResource(string Name, string KindOfResource);