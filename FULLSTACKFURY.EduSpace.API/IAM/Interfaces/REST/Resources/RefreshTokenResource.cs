using System.ComponentModel.DataAnnotations;

namespace FULLSTACKFURY.EduSpace.API.IAM.Interfaces.REST.Resources;

public record RefreshTokenResource([Required] string RefreshToken);
