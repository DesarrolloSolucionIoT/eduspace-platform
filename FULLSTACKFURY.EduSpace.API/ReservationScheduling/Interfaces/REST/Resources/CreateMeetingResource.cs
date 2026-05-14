using System.ComponentModel.DataAnnotations;

namespace FULLSTACKFURY.EduSpace.API.ReservationScheduling.Interfaces.REST.Resources;

public record CreateMeetingResource(
    [Required, StringLength(200, MinimumLength = 1)] string Title,
    [Required] string Description,
    [Required] DateOnly Date,
    [Required] TimeOnly Start,
    [Required] TimeOnly End
);
