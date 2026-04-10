namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Resource;

public record UpdateResourceCommand(
    int Id,
    string Name,
    string KindOfResource,
    int ClassroomId
);