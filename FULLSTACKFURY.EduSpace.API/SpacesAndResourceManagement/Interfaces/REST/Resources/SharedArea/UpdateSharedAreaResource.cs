using System.ComponentModel.DataAnnotations;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.SharedArea;

public record UpdateSharedAreaResource(
    int Id,
    [Required] string Name,
    [Range(1, 1000)] int Capacity,
    [Required] string Description,
    [StringLength(64)] string? ZoneId = null
);
