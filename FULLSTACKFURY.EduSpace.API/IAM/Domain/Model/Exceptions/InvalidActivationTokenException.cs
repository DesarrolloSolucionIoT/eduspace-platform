namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;

public class InvalidActivationTokenException(string message = "El enlace de activación no es válido.") : Exception(message);
