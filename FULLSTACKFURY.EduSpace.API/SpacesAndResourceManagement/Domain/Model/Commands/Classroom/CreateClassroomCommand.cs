namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Classroom;

/// <summary>
///     Command to create a classroom.
/// </summary>
/// <param name="Name">
///     The name of the classroom to create.
/// </param>
/// <param name="Description">
///     The description of the classroom to create.
/// </param>
/// <param name="TeacherId">
///     The ID of the teacher for the classroom.
/// </param>
public record CreateClassroomCommand(string Name, string Description, int TeacherId);