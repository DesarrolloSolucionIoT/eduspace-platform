using System.ComponentModel.DataAnnotations;

namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Resources;

public record VerifyCodeResource(
    [Required] string Username,
    [Required, RegularExpression(@"^\d{6}$", ErrorMessage = "Code must be exactly 6 digits.")]
    string Code
);
