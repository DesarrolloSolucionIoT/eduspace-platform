namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;

public class InvalidPasswordResetTokenException(string message = "El enlace de recuperación no es válido.") : Exception(message);
