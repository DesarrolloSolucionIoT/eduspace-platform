using System.ComponentModel.DataAnnotations;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Classroom;

public record UpdateClassroomResource(
    int Id,
    [Required, StringLength(100)] string Name,
    [Required, StringLength(500)] string Description,
    [Range(1, int.MaxValue)] int TeacherId
);
