namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Classroom;

public record UpdateClassroomCommand(
    int ClassroomId,
    string Name,
    string Description,
    int TeacherId
);