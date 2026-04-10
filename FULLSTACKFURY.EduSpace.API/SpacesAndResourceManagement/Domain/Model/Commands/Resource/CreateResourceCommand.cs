namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Resource;

/// <summary>
///     Command to create a resource.
/// </summary>
/// <param name="Name">
///     The name of the resource to create.
/// </param>
/// <param name="KindOfResource">
///     The kind of resource to create.
/// </param>
/// <param name="ClassroomId">
///     The id of the classroom to create.
/// </param>
public record CreateResourceCommand(string Name, string KindOfResource, int ClassroomId);