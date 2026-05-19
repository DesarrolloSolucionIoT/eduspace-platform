namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;

public class ActivationTokenExpiredException(string message = "El enlace de activación expiró. Pedí uno nuevo.") : Exception(message);
