namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;

public class RefreshTokenAlreadyUsedException(string message = "Refresh token has already been used or revoked.") : Exception(message);
