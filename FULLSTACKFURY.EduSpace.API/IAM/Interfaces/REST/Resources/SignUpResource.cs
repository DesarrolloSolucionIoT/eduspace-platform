using System.ComponentModel.DataAnnotations;

namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Resources;

public record SignUpResource(
    [Required] string Username,
    [Required, StringLength(int.MaxValue, MinimumLength = 8),
     RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{8,}$",
         ErrorMessage = "Password must be at least 8 characters and contain both letters and numbers.")]
    string Password,
    [Required] string Role
);
