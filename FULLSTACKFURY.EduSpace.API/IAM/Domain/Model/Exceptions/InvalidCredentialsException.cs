namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Exceptions;

public class InvalidCredentialsException(string message = "Invalid username or password.") : Exception(message);
