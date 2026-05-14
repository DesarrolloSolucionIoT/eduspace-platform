using System.ComponentModel.DataAnnotations;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Resource;

/// <summary>Payload required to create a new resource inside a classroom.</summary>
public record CreateResourceResource(
    [Required] string Name,
    [Required] string KindOfResource
);
