using System.ComponentModel.DataAnnotations;

namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Resources;

public record SignInResource(
    [Required] string Username,
    [Required] string Password
);
