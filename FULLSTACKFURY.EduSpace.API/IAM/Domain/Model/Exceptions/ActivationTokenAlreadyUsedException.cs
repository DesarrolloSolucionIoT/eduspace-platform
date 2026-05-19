namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;

public class ActivationTokenAlreadyUsedException(string message = "Este enlace ya fue usado.") : Exception(message);
