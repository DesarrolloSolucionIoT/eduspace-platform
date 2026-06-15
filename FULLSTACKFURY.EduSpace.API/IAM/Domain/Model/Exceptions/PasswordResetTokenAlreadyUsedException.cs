namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;

public class PasswordResetTokenAlreadyUsedException(string message = "Este enlace de recuperación ya fue usado.") : Exception(message);
