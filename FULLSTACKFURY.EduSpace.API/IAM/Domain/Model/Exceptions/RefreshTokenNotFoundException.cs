namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;

public class RefreshTokenNotFoundException(string message = "Refresh token not found.") : Exception(message);
