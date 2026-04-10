namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Classroom;

/// <summary>
///     Represents the data required to create a new classroom.
/// </summary>
/// <param name="Name">
///     The name of the classroom
/// </param>
/// <param name="Description">
///     The summary of the Classroom
/// </param>
/// <param name="TeacherId">
///     The Teacher identifier of the classroom
/// </param>
public record CreateClassroomResource(string Name, string Description);