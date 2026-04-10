namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Resource;

public record UpdateResourceResource(
    int Id,
    string Name,
    string KindOfResource,
    int ClassroomId
);