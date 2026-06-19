using System.ComponentModel.DataAnnotations;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Classroom;

/// <summary>Payload required to create a new classroom.</summary>
public record CreateClassroomResource(
    [Required, StringLength(100)] string Name,
    [Required, StringLength(500)] string Description,
    [StringLength(64)] string? ZoneId = null
);
