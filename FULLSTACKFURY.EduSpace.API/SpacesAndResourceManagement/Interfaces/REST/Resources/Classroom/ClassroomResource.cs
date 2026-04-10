namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Classroom;

/// <summary>
///     Represents the data exposed by the tutorial resource.
/// </summary>
/// <param name="Id">
///     The unique identifier of the tutorial.
/// </param>
/// <param name="Name">
///     The name of the classroom.
/// </param>
/// <param name="Description">
///     The description of the classroom.
/// </param>
/// <param name="TeacherId">
///     The unique identifier of the teacher.
/// </param>
public record ClassroomResource(int Id, string Name, string Description, int TeacherId);