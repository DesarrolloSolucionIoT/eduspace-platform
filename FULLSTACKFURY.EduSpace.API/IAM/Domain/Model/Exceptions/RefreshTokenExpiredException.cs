namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;

public class RefreshTokenExpiredException(string message = "Refresh token has expired.") : Exception(message);
