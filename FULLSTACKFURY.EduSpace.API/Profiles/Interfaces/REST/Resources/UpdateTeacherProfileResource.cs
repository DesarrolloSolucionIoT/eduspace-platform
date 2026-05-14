using System.ComponentModel.DataAnnotations;

namespace FULLSTACKFURY.EduSpace.API.Profiles.Interfaces.REST.Resources;

public record UpdateTeacherProfileResource(
    [Required] string FirstName,
    [Required] string LastName,
    [Required][EmailAddress] string Email,
    [Required][RegularExpression(@"^\d{8}$", ErrorMessage = "DNI must be exactly 8 digits.")] string Dni,
    [Required] string Address,
    [Required][RegularExpression(@"^9\d{8}$", ErrorMessage = "Phone must be 9 digits starting with 9.")] string Phone);
