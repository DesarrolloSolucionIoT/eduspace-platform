namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;

public class PasswordResetTokenExpiredException(string message = "El enlace de recuperación expiró. Pedí uno nuevo.") : Exception(message);
