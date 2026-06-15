using System.ComponentModel.DataAnnotations;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.SharedArea;

public record ReserveSharedAreaResource(
    [Required] int TeacherId,
    [Required] DateOnly ReservationDate,
    [Required] TimeOnly StartTime,
    [Required] TimeOnly EndTime,
    [Required] string Reason
    
);