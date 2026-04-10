namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Classroom;

public record UpdateClassroomResource(
    int Id,
    string Name,
    string Description,
    int TeacherId
);