using System.ComponentModel.DataAnnotations;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Resource;

public record UpdateResourceResource(
    int Id,
    [Required] string Name,
    [Required] string KindOfResource,
    [Range(1, int.MaxValue)] int ClassroomId
);
